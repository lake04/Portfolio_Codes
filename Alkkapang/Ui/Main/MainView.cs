using Spine.Unity;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainView : ViewBase,IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    public Scrollbar scrollbar;
    public Transform contentTr;

    public Slider tabSlider;
    public RectTransform[] btnRects, btnImageRect;
    [SerializeField] private Button[] btnImgs;
    [SerializeField] private TMP_Text[] btnTexts;
    [SerializeField] private Sprite orignalSprite;
    [SerializeField] private Sprite selelctSprite;

    public Action<float> OnBeginDragEvent;
    public Action<float, Vector2> OnEndDragEvent;
    public Action<int> OnTabBtnClicked;
    public Action OnSettingButton;

    [HideInInspector] public int targetIndex = 2;
    [HideInInspector] public float targetPos = 2;
    [HideInInspector] public bool isDrag;
    private int currentIndex = -1;
    [SerializeField] private TMP_Text ncinknameText;

    private bool isAnimating;


    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke(scrollbar.value);
    }

    public void OnDrag(PointerEventData eventData) => isDrag = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        OnEndDragEvent?.Invoke(scrollbar.value, eventData.delta);
    }


    public void SetNickname(string nickname)
    {
        if (ncinknameText != null)
        {
            ncinknameText.text = nickname;
        }
    }

    public void RenderTabState(int index, float pos)
    {
        bool changed = currentIndex != index;

        currentIndex = index;
        this.targetIndex = index;
        this.targetPos = pos;

        if (changed)
        {
            UpdateStaticUI();

            if (index == 1)
            {
                RefreshCharacterList();
            }

        }
        isAnimating = true;
    }

    private void UpdateStaticUI()
    {
        for (int i = 0; i < btnRects.Length; i++)
        {
            bool isTarget = (i == targetIndex);

            btnRects[i].sizeDelta = new Vector2(isTarget ? 360 : 180, btnRects[i].sizeDelta.y);
            btnImgs[i].image.sprite = isTarget ? selelctSprite : orignalSprite;
            btnTexts[i].gameObject.SetActive(isTarget);
        }
    }

    void Update()
    {
        if (isDrag || !isAnimating) return;

        bool allLerpFinished = true;

        if (Mathf.Abs(scrollbar.value - targetPos) > 0.001f)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
            tabSlider.value = scrollbar.value;
            allLerpFinished = false;
        }
        else
        {
            scrollbar.value = targetPos;
            tabSlider.value = targetPos;
        }
      
        for (int i = 0; i < btnRects.Length; i++)
        {
            Vector3 btnTargetPos = btnRects[i].anchoredPosition3D;
            btnTargetPos.y = (i == targetIndex) ? -5f : -57f;
            Vector3 btnTargetScale = (i == targetIndex) ? new Vector3(1.2f, 1.2f, 1) : new Vector3(0.9f, 0.9f, 1);

            if (Vector3.Distance(btnImageRect[i].anchoredPosition3D, btnTargetPos) > 0.1f ||
                Vector3.Distance(btnImageRect[i].localScale, btnTargetScale) > 0.01f)
            {
                btnImageRect[i].anchoredPosition3D = Vector3.Lerp(btnImageRect[i].anchoredPosition3D, btnTargetPos, 0.25f);
                btnImageRect[i].localScale = Vector3.Lerp(btnImageRect[i].localScale, btnTargetScale, 0.25f);
                allLerpFinished = false;
            }
            else
            {
                btnImageRect[i].anchoredPosition3D = btnTargetPos;
                btnImageRect[i].localScale = btnTargetScale;
            }
        
        }

        if (allLerpFinished)
        {
            isAnimating = false;
        }
    }

    public void OnClickTab(int index) => OnTabBtnClicked?.Invoke(index);

    public void RefreshCharacterList()
    {
        OnTabBtnClicked?.Invoke(1);
        CharacterUiInstaller.Instance._presenter.RefreshCharacterList();
    }

    public void SetHomeCharacterObject()
    {
        OnTabBtnClicked?.Invoke(2);
    }

    

}
