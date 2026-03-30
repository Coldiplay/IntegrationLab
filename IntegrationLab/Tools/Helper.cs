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

    public static T ChangeAllProperties<T>(T oldObj, T newObj)
    where T : class
    {
        var type = typeof(T);
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            if (property.CanWrite)
            {
                property.SetValue(oldObj, property.GetValue(newObj));
            }
        }
        
        return oldObj;
    }
    
}