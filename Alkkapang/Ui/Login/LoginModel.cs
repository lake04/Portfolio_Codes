using LitJson;
using BackEnd;
using CustomBackEnd.BackendLogin;
using System;   
using UnityEngine;

public class LoginModel : ModelBase
{
    [SerializeField] private string id;
    [SerializeField] private string pw;

    public  void Login(string id, string pw, Action<bool, string> onResult)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            onResult?.Invoke(false, "Please enter ID and PW.");
            return;
        }

        var bro = BackendLogin.Instance.CustomLogin(id, pw);
        Debug.Log($"{id}, {pw} login try...");
        if (bro.IsSuccess())
        {
            onResult?.Invoke(true, "login success");
        }
        else
        {
            string message = bro.GetMessage();
            onResult?.Invoke(false, "login failed: " + message);
        }
    }

    public void SignUp(string id, string pw, Action<bool, string> onResult)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            onResult?.Invoke(false, "ID 또는 PW를 입력해주세요.");
            return;
        }

        var bro = BackendLogin.Instance.CustomSignUp(id, pw);
        if (bro.IsSuccess())
        {
            onResult?.Invoke(true, "회원가입 성공");
        }
        else
        {
            onResult?.Invoke(false, "회원가입 실패: " + bro.GetMessage());
        }
    }

    public void FindPw(string email, Action<bool, string> onResult)
    {
        var bro = BackendLogin.Instance.FindPw(email, email);

        if (bro.IsSuccess())
        {
            onResult?.Invoke(true, "비밀번호 찾기 성공");
        }
        else
        {
            onResult?.Invoke(false, "비밀번호 찾기 실패: " + bro);
        }
    }

    public void UpdatePw(string email, string tempPw, string newPw, Action<bool, string> onResult)
    {
        var bro2 = BackendLogin.Instance.CustomLogin(email, tempPw);
        Debug.Log($"{id}, {pw} 로그인 시도 중...");
        if (bro2.IsSuccess())
        {
            Debug.Log("로그인 성공");
            var bro = Backend.BMember.UpdatePassword(tempPw, newPw);

            if (bro.IsSuccess())
            {
                onResult?.Invoke(true, "비밀번호 업데이트 성공");
            }
            else
            {
                onResult?.Invoke(false, "비밀번호 업데이트 실패: " + bro.GetMessage());
            }
        }
        else
        {
            onResult?.Invoke(false,"로그인 실패" + bro2.Message);
        }
    }

    public void UpdateNickname(string nickname, Action<bool, string> onResult)
    {
        var bro = BackendLogin.Instance.UpdateNickname(nickname);
        if (bro.IsSuccess())
        {
            onResult?.Invoke(true, "닉네임 업데이트 성공");
        }
        else
        {
            onResult?.Invoke(false, "닉네임 업데이트 실패: " + bro.GetMessage());
        }
    }

    public void GoogleLogin(Action<bool, string> onResult)
    {
        BackendLogin.Instance.StartGoogleLogin(onResult);
    }

    public void CheckHasNickname(Action<bool, bool, string> onResult)
    {
        var bro = Backend.BMember.GetUserInfo();
        if (bro.IsSuccess())
        {
            JsonData json = JsonMapper.ToObject(bro.GetReturnValue());
            JsonData row = json["row"];

            if (row.Keys.Contains("nickname") && row["nickname"] != null && !string.IsNullOrEmpty(row["nickname"].ToString()))
            {
                onResult?.Invoke(true, true, "닉네임 존재함");
            }
            else
            {
                onResult?.Invoke(true, false, "닉네임 없음");
            }
        }
        else
        {
            onResult?.Invoke(false, false, "유저 정보 조회 실패: " + bro.GetMessage());
        }
    }
}
