using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailView : ViewBase
{
    [Header("Info")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text characterRarityText;
    [SerializeField] private Image characterRarityImage;

    [SerializeField] private Sprite[] rarityImages;
    [SerializeField] private TMP_Text characterDescriptionText;

    [Header("Stat")]
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


        switch(data.rarity)
        {
            case 1:
                characterRarityText.text = "ÀÏ¹Ý";
                break;
            case 2:
                characterRarityText.text = "Èñ±Í";
                break;
            case 3:
                characterRarityText.text = "¿µ¿õ";
                break;
            case 4:
                characterRarityText.text = "Àü¼³";
                break;
            case 5:
                characterRarityText.text = "¾Ë±îÆÎ";
                break;
            default:
                break;
        }

        characterRarityImage.sprite = rarityImages[data.rarity - 1];
        characterImage.preserveAspect = true;

        characterDescriptionText.text = uiData.desc;

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
