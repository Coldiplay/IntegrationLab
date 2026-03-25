using System;
using Avalonia.Controls;
using IntegrationLab.ViewModels;

namespace IntegrationLab.Tools;

public static class Helper
{
    public static TControl InitializeView<TControl>()
    where TControl : Control, new()
    {
        var control = new TControl();
        
        var vmTypeName = control.GetType().FullName!.Replace("View", "ViewModel", StringComparison.Ordinal);
        var vmType = Type.GetType(vmTypeName);
        var vm = Activator.CreateInstance(vmType!) as ViewModelControlBase<TControl>;
        
        vm!.View = control;
        vm.OnCreating();
        
        control.DataContext = vm;
        return control;
    }
}