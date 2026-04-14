using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendView : ViewBase
{
    private FriendPresenter _presenter;

    [SerializeField] private Button friendOpenButtom;
    [SerializeField] private Button friendCloseButtom;

    public Action OnClickOpenFirend;
    public Action OnClickCloseFirend;

    [SerializeField] private GameObject friendPopup;


    private void Awake()
    {
        var model = new FriendModel();

        _presenter = new FriendPresenter(this, model);
    }

    void Start()
    {
        friendOpenButtom.onClick.AddListener(() => OnClickOpenFirend?.Invoke());
        friendCloseButtom.onClick.AddListener(() => OnClickCloseFirend?.Invoke());
    }

    void Update()
    {
        
    }

    public void FriendPopup()
    {
        UiManager.instance.FriendPopup(friendPopup);
    }
}
