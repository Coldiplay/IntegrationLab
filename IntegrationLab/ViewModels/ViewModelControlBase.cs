using Avalonia.Controls;

namespace IntegrationLab.ViewModels;

public abstract class ViewModelControlBase<TControl> : ViewModelBase
    where TControl : Control
{
    protected ViewModelControlBase()
    {
        //View = (TControl)App.Services.GetRequiredService<ViewLocator>().Build(this);
    }

    protected ViewModelControlBase(TControl view)
    {
        View = view;
    }

    public virtual void OnCreating()
    {
    }


    public new TControl View;
}