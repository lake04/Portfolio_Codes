using System.Text.RegularExpressions;
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

        View.OnClickGoogleLogin += GoogleLogin;

        View.OnClickOpenSignUp += OpenSign;
        View.OnClickCloseSignUp += HideSign;
        View.OnClickSignUp += OnSign;

        View.OnClickOpenFindEmail += OpenFindEmail;
        View.OnClickUpdatePw += OnUpdatePw;
        View.OnClickFindPw += FindEmail;
        View.OnClickCloseEmail += HideEmail;

        View.OnClickNickName += UpdateNickname;
    }

    public override void OnDestroy()
    {
        View.OnClickOpenLogin -= OpenLogin;
        View.OnClickLogin -= OnLogin;

        View.OnClickGoogleLogin -= GoogleLogin;

        View.OnClickOpenSignUp -= OpenSign;
        View.OnClickCloseSignUp -= HideSign;
        View.OnClickSignUp -= OnSign;

        View.OnClickOpenFindEmail -= OpenFindEmail;
        View.OnClickUpdatePw -= OnUpdatePw;
        View.OnClickFindPw -= FindEmail;
        View.OnClickCloseEmail -= HideEmail;

        View.OnClickNickName -= UpdateNickname;
    }
   

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    private void OnLogin()
    {
        string id = View.GetId();
        string pw = View.GetPassword();

        View.ShowIdError(false);
        View.ShowPwError(false);

        if (string.IsNullOrEmpty(id))
        {
            View.ShowIdError(true, "이메일을 입력해주세요.");
            return;
        }

        if (!IsValidEmail(id))
        {
            View.ShowIdError(true, "잘못된 이메일 형식입니다.");
            return;
        }

        if (string.IsNullOrEmpty(pw))
        {
            View.ShowPwError(true);
            return;
        }

        Model.Login(id, pw, (success, msg) =>
        {
            View.ShowMessage(msg);
            if (!success)
            {
                View.ShowIdError(true, "아이디 또는 비밀번호가 틀렸습니다.");
                View.ShowPwError(true);
                return;
            }
            CheckAndEnterMainScene();
        });
    }

    private void OpenLogin()
    {
        View.ShowLoginPop();
    }

    private void GoogleLogin()
    {
        Model.GoogleLogin((success, msg) =>
        {
            View.ShowMessage(msg);
            if (!success) return;

            CheckAndEnterMainScene(); 
        });
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
            if (!success) return;
            CheckAndEnterMainScene(); 
        });
    }

    private void CheckAndEnterMainScene()
    {
        Model.CheckHasNickname((isSuccess, hasNickname, msg) =>
        {
            if (isSuccess)
            {
                if (hasNickname)
                {
                    EnterMainScene();
                }
                else
                {
                    View.ShowNickname();
                }
            }
            else
            {
                View.ShowMessage("닉네임 검사 실패: " + msg);
            }
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

    private void UpdateNickname()
    {
        string nickname = View.GetNickname();
        Model.UpdateNickname(nickname, (success, msg) =>
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