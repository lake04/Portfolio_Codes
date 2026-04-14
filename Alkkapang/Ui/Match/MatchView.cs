using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchView : ViewBase
{
    [Header("Buttons")]
    [SerializeField] private Button matchButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text matchText;

    [Header("Objects")]
    [SerializeField] private GameObject matchingOb;

    public Action OnClickMatch;

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
}