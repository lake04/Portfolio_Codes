using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchView : ViewBase
{
    [Header("Buttons")]
    [SerializeField] private Button matchButton;
    public Button[] gameRuleButtons;
    [SerializeField] private Button snowGameRuleButton;
    [SerializeField] private Button hideButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text matchText;

    [Header("Objects")]
    [SerializeField] private GameObject matchingOb;
    [SerializeField] private GameObject gameRuleOb;

    public Action OnClickMatch;
    public Action<int> OnClickSelectRule;
    public Action OnClickSnowGameRule;
    public Action OnClickHide;

    private MatchPresenter _presenter;
    private Coroutine matchingCoroutine;


    private void Awake()
    {
        var matchModel = new MatchModel();
        _presenter = new MatchPresenter(this, matchModel);
    }

    private void Start()
    {
        matchButton.onClick.AddListener(() => OnClickMatch?.Invoke());

        for (int i = 0; i < gameRuleButtons.Length; i++)
        {
            int index = i;
            gameRuleButtons[i].onClick.AddListener(() =>
            {
                OnClickSelectRule?.Invoke(index);
            });
        }

        snowGameRuleButton.onClick.AddListener(() => OnClickSnowGameRule?.Invoke());
        hideButton.onClick.AddListener(() => OnClickHide?.Invoke());
    }

    private void OnDestroy()
    {
        _presenter?.OnDestroy();
    }


    public void ShowMatchingUI(bool isShow)
    {
        matchingOb.SetActive(isShow);

        if (isShow)
        {
            if (matchingCoroutine == null)
            {
                matchingCoroutine = StartCoroutine(MatchingRoutine());
            }
        }
        else
        {
            if (matchingCoroutine != null)
            {
                StopCoroutine(matchingCoroutine);
                matchingCoroutine = null;
            }

            matchText.text = string.Empty;
        }
    }

    private IEnumerator MatchingRoutine()
    {
        const string baseText = "매칭 상대를 찾는 중입니다";

        while (true)
        {
            for (int i = 0; i <= 3; i++)
            {
                matchText.text = baseText + new string('.', i);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void SnowGameRule()
    {
        UiManager.instance.Popup(gameRuleOb);
    }

    public void HideGameRule()
    {
        UiManager.instance.CloseTopUi();
    }
        
}