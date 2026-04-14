using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : ViewBase
{
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button openSettingButton;
    [SerializeField] private Button closeSettingButton;

    public Action OnClickLogout;
    public Action OnClickSetting;
    public Action OnCloseSetting;

    private SettingPresenter _presenter;

    [SerializeField] private GameObject settingpopup;

    private void Awake()
    {
        var model = new SettingModel();

        _presenter = new SettingPresenter(this, model);
    }

    void Start()
    {
        logoutButton.onClick.AddListener(() => OnClickLogout?.Invoke());
        openSettingButton.onClick.AddListener(() => OnClickSetting?.Invoke());
        closeSettingButton.onClick.AddListener(() => OnCloseSetting?.Invoke());
    }

    public void OpenSetting()
    {
        UiManager.instance.Popup(settingpopup);
    }

    public void CloseSetting()
    {
        UiManager.instance.CloseTopUi();
    }
}
