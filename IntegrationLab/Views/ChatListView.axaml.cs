using Avalonia.Controls;
using Avalonia.Input;
using IntegrationLab.ViewModels;

namespace IntegrationLab.Views;

public partial class ChatListView : UserControl
{
    public ChatListView()
    {
        InitializeComponent();
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e) =>
        (DataContext as ChatListViewModel)!.OnDoubleTapped(sender, e);
}