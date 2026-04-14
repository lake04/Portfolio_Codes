using System;

public class MainPresenter : PresenterBase<MainView, MainModel>
{
    public MainPresenter(MainView view, MainModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnInitialize()
    {

        View.OnTabBtnClicked += (index) => Model.UpdateTargetByIndex(index);
        View.OnEndDragEvent += (val, delta) => Model.UpdateTargetByScrollValue(val, delta.x);

        Model.OnTabStateChanged += (index, pos) =>
        {
            View.RenderTabState(index, pos);
        };
    }

    public override void OnDestroy()
    {
        View.OnEndDragEvent = null;
        View.OnTabBtnClicked = null;
        Model.OnTabStateChanged = null;
    }

    
}