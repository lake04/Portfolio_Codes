using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : ViewBase
{
    [Header("Login")]
    [SerializeField] private GameObject loginOb;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField loginPwInputField;
    [SerializeField] private Button loginSelectButton;

    [Header("SignUp")]
    [SerializeField] private GameObject signOb;
    [SerializeField] private TMP_InputField signInputField;
    [SerializeField] private TMP_InputField pwInputField;
    [SerializeField] private Button signupOpenButton;
    [SerializeField] private Button signupButton;
    [SerializeField] private Button signupCloseButton;

    [Header("Google Login")]
    [SerializeField] private GameObject googleLogin;
    [SerializeField] private Button googleLoginButton;

    [Header("FindPw")]
    [SerializeField] private GameObject findOb;
    [SerializeField] private GameObject findEmailOb;
    [SerializeField] private Button openEmailButton;
    [SerializeField] private Button findEmailButton;
    [SerializeField] private Button closeEmailButton;
    [SerializeField] private TMP_InputField findEmailInputField;
    [SerializeField] private TMP_InputField temporaryPwInputField;
    [SerializeField] private TMP_InputField newPwInputField;
    [SerializeField] private Button updatePwButton;
    [SerializeField] private Button closeFindEmailButton;

    public Action OnClickOpenLogin;
    public Action OnClickLogin;

    public Action OnClickGoogleLogin;

    public Action OnClickOpenSignUp;
    public Action OnClickSignUp;
    public Action OnClickCloseSignUp;
    public Action OnClickOpenFindEmail;
    public Action OnClickFindPw;
    public Action OnClickUpdatePw;
    public Action OnClickCloseEmail;
    public Action OnClickCloseFindEmail;

    #region 인풋필드 텍스트 읽기
    public string GetId() => idInputField.text;
    public string GetPassword() => loginPwInputField.text;

    public string GetSignId() => signInputField.text;
    public string GetSignPw() => pwInputField.text;

    public string GetFindEmail() => findEmailInputField.text;
    public string GetTempPw() => temporaryPwInputField.text;
    public string GetNewPw() => newPwInputField.text;
    #endregion

    private LoginPresenter _presenter;

    private void Awake()
    {
        var model = new LoginModel();
        _presenter = new LoginPresenter(this, model);
    }

    private void Start()
    {
        loginSelectButton.onClick.AddListener(() => OnClickLogin?.Invoke());
        signupOpenButton.onClick.AddListener(() => OnClickOpenSignUp?.Invoke());

        googleLoginButton.onClick.AddListener(() => OnClickGoogleLogin?.Invoke());

        signupButton.onClick.AddListener(() => OnClickSignUp?.Invoke());
        signupCloseButton.onClick.AddListener(() => OnClickCloseSignUp?.Invoke());

        openEmailButton.onClick.AddListener(() => OnClickOpenFindEmail?.Invoke());
        findEmailButton.onClick.AddListener(() => OnClickFindPw?.Invoke());
        updatePwButton.onClick.AddListener(() => OnClickUpdatePw?.Invoke());
        closeEmailButton.onClick.AddListener(() => OnClickCloseEmail?.Invoke());
        closeFindEmailButton.onClick.AddListener(() => OnClickCloseFindEmail?.Invoke());
    }

    private void OnDestroy()
    {
        _presenter?.OnDestroy();
    }

    public void ShowLoginPop()
    {
        loginOb.SetActive(true);
    }

    public void ShowEmail()
    {
        loginOb.SetActive(false);
        signOb.SetActive(false);
        findEmailOb.SetActive(false);
        findOb.SetActive(true);
    }

    public void ShowFindEmail()
    {
        findOb.SetActive(false);
        findEmailOb.SetActive(true);
    }

    public void ShowSignup()
    {
        loginOb.SetActive(false);
        findOb.SetActive(false);
        findEmailOb.SetActive(false);
        signOb.SetActive(true);
    }

    public void ShowMessage(string msg)
    {
        Debug.Log(msg);
    }

    public void HideEmail()
    {
        loginOb.SetActive(true);
        findOb.SetActive(false);
    }

    public void HideFindEmail()
    {
        loginOb.SetActive(true);
        findEmailOb.SetActive(false);
    }

    public void HideSign()
    {
        signOb.SetActive(false);
        loginOb.SetActive(true);
    }
}