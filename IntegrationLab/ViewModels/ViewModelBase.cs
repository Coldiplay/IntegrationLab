using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IntegrationLab.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public Control? View;
}