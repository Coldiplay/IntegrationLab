using Avalonia.Controls;
using Avalonia.Input;
using IntegrationLab.ViewModels;

namespace IntegrationLab.Views;

public partial class IncidentsView : UserControl
{
    public IncidentsView()
    {
        InitializeComponent();
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e) =>
        (DataContext as IncidentsViewModel)!.OnDoubleTapped(sender, e);
}