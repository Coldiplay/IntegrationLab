using Avalonia.Controls;
using IntegrationLab.ViewModels;

namespace IntegrationLab.Tools;

public static class Helper
{
    public static TControl InitializeView<TControl, TViewModel>()
    where TControl : Control, new()
    where TViewModel : ViewModelControlBase<TControl>, new()
    {
        var control = new TControl();
        var vm = new TViewModel
        {
            View = control
        };
        control.DataContext = vm;
        
        return control;
    }
}