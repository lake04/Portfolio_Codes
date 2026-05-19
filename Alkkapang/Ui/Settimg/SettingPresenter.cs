using CustomBackEnd.BackendLogin;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        BackendLogin.Instance.Logout();
        SceneManager.LoadScene("Title");
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
