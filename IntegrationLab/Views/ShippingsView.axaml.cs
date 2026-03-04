using Avalonia.Controls;

namespace IntegrationLab.Views;

public partial class ShippingsView : UserControl
{
    public ShippingsView()
    {
        InitializeComponent();
    }

    // private void Button_OnClick(object? sender, RoutedEventArgs e)
    // {
    //     var test = ((sender as Button)!.Parent as Grid).Children
    //         .Where(c => c is Button).Cast<Button>().ToList();
    //     var vm = this.DataContext as ShippingsViewModel;
    //     test[0].Command = vm.ConfirmShippingCommand;
    //     var test2 = test[0].IsEnabled;
    //     (test[0].Command as RelayCommand).NotifyCanExecuteChanged();
    //     var test3 = test[0].IsEnabled;
    //     test[0].Command.Execute(null);
    // }
}