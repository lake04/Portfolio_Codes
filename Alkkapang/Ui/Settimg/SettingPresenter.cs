using CustomBackEnd.BackendLogin;
using UnityEngine;

public class SettingPresenter : PresenterBase<SettingView,SettingModel>
{
    public  SettingPresenter(SettingView view, SettingModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnDestroy()
    {
        View.OnClickLogout -= Logout;
        View.OnClickSetting -= OpenSettingpopup;
        View.OnCloseSetting -= CloseSettingpopup;
    }

    public override void OnInitialize()
    {
        View.OnClickLogout += Logout;
        View.OnClickSetting += OpenSettingpopup;
        View.OnCloseSetting += CloseSettingpopup;
    }

    private void Logout()
    {
        Debug.Log("Logout");
        //BackendLogin.Instance.Logout();
    }

    private void OpenSettingpopup()
    {
        View.OpenSetting();
    }

    private void CloseSettingpopup()
    {
        View.CloseSetting();
    }
    
}
