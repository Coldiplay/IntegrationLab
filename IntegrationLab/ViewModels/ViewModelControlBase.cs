using Avalonia.Controls;

namespace IntegrationLab.ViewModels;

public class ViewModelControlBase<TControl> : ViewModelBase
where TControl : Control
{
    public ViewModelControlBase() {}

    public ViewModelControlBase(TControl view)
    {
        View = view;
    }
    
    
    public TControl View;
}