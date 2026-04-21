using UnityEditor.Media;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginPresenter : PresenterBase<LoginView, LoginModel>
{
    public LoginPresenter(LoginView view, LoginModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnInitialize()
    {
        View.OnClickOpenLogin += OpenLogin;
        View.OnClickLogin += OnLogin;

        View.OnClickGoogleLogin += GogleLogin;

        View.OnClickOpenSignUp += OpenSign;
        View.OnClickCloseSignUp += HideSign;
        View.OnClickSignUp += OnSign;

        View.OnClickOpenFindEmail += OpenFindEmail;
        View.OnClickUpdatePw += OnUpdatePw;
        View.OnClickFindPw += FindEmail;
        View.OnClickCloseEmail += HideEmail;
    }

    public override void OnDestroy()
    {
        View.OnClickOpenLogin -= OpenLogin;
        View.OnClickLogin -= OnLogin;

        View.OnClickGoogleLogin -= GogleLogin;

        View.OnClickOpenSignUp -= OpenSign;
        View.OnClickCloseSignUp -= HideSign;
        View.OnClickSignUp -= OnSign;

        View.OnClickOpenFindEmail -= OpenFindEmail;
        View.OnClickUpdatePw -= OnUpdatePw;
        View.OnClickFindPw -= FindEmail;
        View.OnClickCloseEmail -= HideEmail;
    }

    private void OpenLogin()
    {
        View.ShowLoginPop();
    }

    private void OnLogin()
    {
        string id = View.GetId();
        string pw = View.GetPassword();

        Model.Login(id, pw, (success, msg) =>
        {
            View.ShowMessage(msg);

            if (!success)
                return;

            EnterMainScene();
        });
    }


    private void GogleLogin()
    {
        Model.GogleLogin();
    }
    private void OpenSign()
    {
        View.ShowSignup();
    }

    private void OnSign()
    {
        string id = View.GetSignId();
        string pw = View.GetSignPw();

        Model.SignUp(id, pw, (success, msg) =>
        {
            View.ShowMessage(msg);

            if (!success)
                return;

            EnterMainScene();
        });
    }

    private void HideSign()
    {
        View.HideSign();
    }

    private void OpenFindEmail()
    {
        View.ShowEmail();
    }

    private void FindEmail()
    {
        string email = View.GetFindEmail();

        Model.FindPw(email, (success, msg) =>
        {
            View.ShowMessage(msg);

            if (!success)
                return;

            View.ShowFindEmail();
        });
    }

    private void OnUpdatePw()
    {
        string email = View.GetFindEmail();
        string tempPw = View.GetTempPw();
        string newPw = View.GetNewPw();

        Model.UpdatePw(email, tempPw, newPw, (success, msg) =>
        {
            View.ShowMessage(msg);

            if (!success)
                return;

            EnterMainScene();
        });
    }

    private void HideEmail()
    {
        View.HideEmail();
    }

    private void HideFindEmail()
    {
        View.HideFindEmail();
    }

    private void EnterMainScene()
    {
        BackEndMatchManager.Instance.GetMatchListFromServer((ok) =>
        {
            if (!ok)
            {
                View.ShowMessage("매치 리스트를 불러오지 못했습니다.");
                return;
            }

            SceneManager.LoadScene("MainScene");
        });
    }
}