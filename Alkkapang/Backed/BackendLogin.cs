    using BackEnd;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    namespace CustomBackEnd.BackendLogin
    {
        using static BackEnd.SendQueue;

        public class BackendLogin
        {
            private static BackendLogin _instance = null;

            public static BackendLogin Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new BackendLogin();
                    }

                    return _instance;
                }
            }

            private Action<bool, string> _onGoogleLoginResult;

            public BackendReturnObject CustomSignUp(string email, string pw)
            {
                Debug.Log("회원가입 요청 중");

                return Backend.BMember.CustomSignUp(email, pw);
            }

            public void RegisterEmail(string email)
            {
                Debug.Log("유저 정보에 이메일 등록 요청: " + email);
                var bro = Backend.BMember.UpdateCustomEmail(email);

                if (bro.IsSuccess())
                {
                    Debug.Log("이메일 등록 성공");
                }
                else
                {
                    Debug.LogError("이메일 등록 실패: " + bro);
                }
            }

            public BackendReturnObject CustomLogin(string id, string pw)
            {
                    Debug.Log("로그인 요청 중...");
                    return Backend.BMember.CustomLogin(id, pw);
            }

            public BackendReturnObject UpdateNickname(string nickName)
            {
                Debug.Log("닉네임 변경 요청");

                return Backend.BMember.UpdateNickname(nickName);

            }

            public void BackendTokenLogin(Action<bool, string> func)
            {
                Enqueue(Backend.BMember.LoginWithTheBackendToken, callback =>
                {
                    if (callback.IsSuccess())
                    {
                        Debug.Log("토큰 로그인 성공");
                        func(true, string.Empty);
                        return;
                    }

                    Debug.Log("토큰 로그인 실패\n" + callback.ToString());
                    func(false, string.Empty);
                });
            }

            public void Logout()
            {
                Debug.Log("로그아웃");
                Backend.BMember.Logout();
            }

            public BackendReturnObject FindPw(string id, string mail)
            {
              return  Backend.BMember.ResetPassword(id, mail);
             
            }

        public void StartGoogleLogin(Action<bool, string> onResult)
        {
            _onGoogleLoginResult = onResult;
            TheBackend.ToolKit.GoogleLogin.Android.GoogleLogin(GoogleLoginCallback);
        }

        public void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
        {
            if (isSuccess == false)
            {
                _onGoogleLoginResult?.Invoke(false, errorMessage);
                return;
            }

            Debug.Log("구글 토큰 발급 성공 : " + token);
            var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google, "GPGS로 가입함");

            if (bro.IsSuccess())
            {
                _onGoogleLoginResult?.Invoke(true, "구글 로그인 성공");
            }
            else
            {
                _onGoogleLoginResult?.Invoke(false, "페더레이션 로그인/가입 에러 : " + bro.GetMessage());
            }
        }

    }
    }
