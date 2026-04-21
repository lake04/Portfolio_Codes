using Photon.Realtime;
using System;
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


    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        tabSlider.value = scrollbar.value;

        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);

            for (int i = 0; i < btnRects.Length; i++)
            {
                btnRects[i].sizeDelta = new Vector2(i == targetIndex ? 360 : 180, btnRects[i].sizeDelta.y);
                btnImgs[i].image.sprite = i == targetIndex ? selelctSprite : orignalSprite;
            }
        }

        if (Time.time < 0.1f) return;

        for (int i = 0; i < btnRects.Length; i++)
        {
            Vector3 btnTargetPos = btnRects[i].anchoredPosition3D;
            btnTargetPos.y = -57;
            Vector3 btnTargetScale = new Vector3(0.9f, 0.9f, 1);

            if (i == targetIndex)
            {
                btnTargetPos.y = -10f;
                btnTargetScale = new Vector3(1f, 1f, 1);
            }

            btnImageRect[i].anchoredPosition3D = Vector3.Lerp(btnImageRect[i].anchoredPosition3D, btnTargetPos, 0.25f);
            btnImageRect[i].localScale = Vector3.Lerp(btnImageRect[i].localScale, btnTargetScale, 0.25f);
        }
    }

    public void RenderTabState(int index, float pos)
    {
        bool changed = currentIndex != index;

        currentIndex = index;
        this.targetIndex = index;
        this.targetPos = pos;

        if (changed && index == 1)
        {
           RefreshCharacterList();
        }
    }

    public void OnClickTab(int index) => OnTabBtnClicked?.Invoke(index);

    public void RefreshCharacterList()
    {
        OnTabBtnClicked?.Invoke(1);
        CharacterUiInstaller.Instance._presenter.RefreshCharacterList();
    }

}
