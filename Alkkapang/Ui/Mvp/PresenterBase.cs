public abstract class PresenterBase<TView, TModel>
    where TView : ViewBase
    where TModel : ModelBase
{
    protected readonly TView View;
    protected readonly TModel Model;

    protected PresenterBase(TView view, TModel model)
    {
        View = view;
        Model = model;
    }

    public abstract void OnInitialize();

    public abstract void OnDestroy();
}