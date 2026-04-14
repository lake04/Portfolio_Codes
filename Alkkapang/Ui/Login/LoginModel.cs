using BackEnd;
using CustomBackEnd.BackendLogin;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TheBackend.ToolKit.GoogleLogin.Android;

public class LoginModel : ModelBase
{
    [SerializeField] private string id;
    [SerializeField] private string pw;

    public  void Login(string id, string pw, Action<bool, string> onResult)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            onResult?.Invoke(false, "ID 또는 PW를 입력해주세요.");
            return;
        }

        var bro = BackendLogin.Instance.CustomLogin(id, pw);
        Debug.Log($"{id}, {pw} 로그인 시도 중...");
        if (bro.IsSuccess())
        {
            onResult?.Invoke(true, "로그인 성공");
            CharacterDataManager.Instance.InitializeDatabase();
        }
        else
        {
            string message = bro.GetMessage();
            onResult?.Invoke(false, "로그인 실패" + message);
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

    public void GogleLogin()
    {
        BackendLogin.Instance.StartGoogleLogin();
       
    }
}
