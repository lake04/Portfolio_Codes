using UnityEngine;

public class FriendPresenter : PresenterBase<FriendView, FriendModel>
{
 
    public FriendPresenter(FriendView view, FriendModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnInitialize()
    {
        View.OnClickOpenFirend += OpenFriendPopup;
        View.OnClickCloseFirend += OpenFriendPopup;

    }
    public override void OnDestroy()
    {
        View.OnClickOpenFirend -= OpenFriendPopup;
        View.OnClickCloseFirend -= OpenFriendPopup;
    }

    private void OpenFriendPopup()
    {
        View.FriendPopup();
    }
}
