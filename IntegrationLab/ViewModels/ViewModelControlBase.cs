using Avalonia.Controls;

namespace IntegrationLab.ViewModels;

public abstract class ViewModelControlBase<TControl> : ViewModelBase
where TControl : Control
{
    protected ViewModelControlBase() {}

    protected ViewModelControlBase(TControl view)
    {
        View = view;
    }

    public virtual void OnCreating()
    {
        
    }
    
    
    public TControl View;
}