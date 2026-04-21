using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterInfoUi : MonoBehaviour
{
    [Header("setting")]
    [SerializeField] private Image capsule;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Image stage;

    private CharacterItemViewData currentData;
    private CharacterUiData currentUiData;
    private Vector3 originSize;
    private bool isOwned;

    private Action OnClicked;

    [SerializeField] private Button selectButton;

    private void Start()
    {
        OnClicked += DetailPopup;
        selectButton?.onClick.AddListener(() => OnClicked?.Invoke());
    }

    public void SetData(CharacterItemViewData data, CharacterUiData uiData)
    {
        currentData = data;
        currentUiData = uiData;
        isOwned = data.isOwned;

        characterName.text = data.masterData.name;
        characterImage.sprite = uiData.character;
        stage.sprite = uiData.stage;
        capsule.sprite = uiData.capsule;

        capsule.gameObject.SetActive(!isOwned);
        stage.gameObject.SetActive(isOwned);
        characterName.gameObject.SetActive(false);

        originSize = transform.localScale;
    }


    private void DetailPopup()
    {
        if (!isOwned) return;
        UiManager.instance.DetailPopup(currentData.masterData.characterId, currentUiData);
    }
}