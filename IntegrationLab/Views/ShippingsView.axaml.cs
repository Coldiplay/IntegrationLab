using Avalonia.Controls;
using Avalonia.Input;
using IntegrationLab.ViewModels;

namespace IntegrationLab.Views;

public partial class ShippingsView : UserControl
{
    public ShippingsView()
    {
        InitializeComponent();
    }
    
    private void OnDoubleTapped(object? sender, TappedEventArgs e) =>
        (DataContext as ShippingsViewModel)!.OnDoubleTapped(sender, e);
}