using UnityEngine;

public class MatchPresenter : PresenterBase<MatchView, MatchModel>
{
    public MatchPresenter(MatchView view, MatchModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnInitialize()
    {
        View.OnClickMatch += OnClickMatch;
        BackEndMatchManager.Instance.OnMatchingStateChanged += View.ShowMatchingUI;
    }

    public override void OnDestroy()
    {
        View.OnClickMatch -= OnClickMatch;

        if (BackEndMatchManager.Instance != null)
        {
            BackEndMatchManager.Instance.OnMatchingStateChanged -= View.ShowMatchingUI;
        }
    }

    private void OnClickMatch()
    {
        Model.StartMatching();
    }
}