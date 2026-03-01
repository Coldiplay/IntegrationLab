using Avalonia.Controls;

namespace IntegrationLab.ViewModels;

public abstract class ViewModelControlBase<TControl> : ViewModelBase
where TControl : Control
{
    public ViewModelControlBase() {}

    public ViewModelControlBase(TControl view)
    {
        View = view;
    }

    public abstract void OnCreating();
    
    
    public TControl View;
}