using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterDetailView : ViewBase
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text weightStatText;
    [SerializeField] private TMP_Text speedStatText;
    [SerializeField] private TMP_Text defenseStatText;
    [SerializeField] private TMP_Text powerStatText;
    [SerializeField] private TMP_Text handlingStatText;

    [SerializeField] private Button selectButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    public Action OnClickSelect;
    public Action OnClickUpgrade;
    public Action OnClickClose;
    public Action<float, Vector2> OnEndDragEvent;

    [HideInInspector] public int targetIndex;
    [HideInInspector] public float targetPos;
    private int currentIndex = -1;

    private CharacterDetailPresenter _presenter;

    [SerializeField] PageIndicator pageIndicator;

    private void Awake()
    {
        var model = new CharacterDetailModel();
        _presenter = new CharacterDetailPresenter(this,model);
    }

    void Start()
    {
        selectButton.onClick.AddListener(() => OnClickSelect?.Invoke());
        upgradeButton.onClick.AddListener(() => OnClickUpgrade?.Invoke());
        closeButton.onClick.AddListener(() => OnClickClose?.Invoke());
    }

    public void SetData(CharacterMasterData data, CharacterUiData uiData)
    {
        _presenter.SetData(data, uiData);
    }

    public void SetCharacterInfo(CharacterMasterData data, CharacterUiData uiData)
    {
        characterImage.sprite = uiData.character;
        characterNameText.text = data.name;

        weightStatText.text =   data.weight.ToString();
        speedStatText.text  =   data.speed.ToString();
        defenseStatText.text =   data.defense.ToString();
        powerStatText.text  =   data.power.ToString();
        handlingStatText.text =  data.handling.ToString();
    }

    public void Close()
    {
        UiManager.instance.CloseTopUi();
    }

    public void RenderTabState(int index, float pos)
    {
        bool changed = currentIndex != index;

        currentIndex = index;
        this.targetIndex = index;
        this.targetPos = pos;
    }

}
