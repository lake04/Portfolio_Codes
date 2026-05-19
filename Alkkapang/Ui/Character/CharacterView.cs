using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : ViewBase
{
    [SerializeField] private CharacterUiData[] characterUiDatas;
    [SerializeField] private GameObject characterItemObject;
    [SerializeField] private Transform  ownedCharacterListGameObject;
    [SerializeField] private Transform  notOwnedCharacterListGameObject;
    [SerializeField] private RectTransform characterList;

    [Header("Button")]
    [SerializeField] private Button sortButton;
    [SerializeField] private Button ascendingButton;

    [Header("Text")]
    [SerializeField] public TMP_Text sortButtonText;

    [Header("Sprite")]
    [SerializeField] private Sprite[] ascendingSprite;

    [Header("Event")]
    public Action onClickSortButton;
    public Action onClickAscendingButton;

    private Dictionary<int, GameObject> _characterCards = new Dictionary<int, GameObject>();

    private void Awake()
    {
        sortButton.onClick.AddListener(() => onClickSortButton?.Invoke());
        ascendingButton.onClick.AddListener(() => onClickAscendingButton?.Invoke());
        Clear();
        LayoutRebuilder.ForceRebuildLayoutImmediate(characterList);
    }

   
    public void Render(List<CharacterItemViewData> sortedList)
    {
        int ownedIndex = 0;
        int notOwnedIndex = 0;

        foreach (var data in sortedList)
        {
            if (!_characterCards.TryGetValue(data.masterData.characterId, out GameObject card))
            {
                card = InstantiateCharacterUi(data);
                _characterCards.Add(data.masterData.characterId, card);
            }
            else if (card.TryGetComponent(out CharacterInfoUi characterUi))
            {
                CharacterUiData uiData = characterUiDatas[data.masterData.characterId];
                characterUi.SetData(data, uiData);
            }

            Transform targetParent = data.isOwned ? ownedCharacterListGameObject : notOwnedCharacterListGameObject;

            if (card.transform.parent != targetParent)
            {
                card.transform.SetParent(targetParent, false);
            }

            if (data.isOwned)
            {
                card.transform.SetSiblingIndex(ownedIndex++);
            }
            else
            {
                card.transform.SetSiblingIndex(notOwnedIndex++);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(characterList);
    }

    private GameObject InstantiateCharacterUi(CharacterItemViewData data)
    {
        GameObject characterCard = Instantiate(characterItemObject);

        if (characterCard.TryGetComponent(out CharacterInfoUi characterUi))
        {
            CharacterUiData uiData = characterUiDatas[data.masterData.characterId];
            characterUi.SetData(data, uiData);
        }

        return characterCard;
    }

    public void Clear()
    {
        for (int i = ownedCharacterListGameObject.childCount - 1; i >= 0; i--)
        {
            Destroy(ownedCharacterListGameObject.GetChild(i).gameObject);
        }

        for (int i = notOwnedCharacterListGameObject.childCount - 1; i >= 0; i--)
        {
            Destroy(notOwnedCharacterListGameObject.GetChild(i).gameObject);
        }
    }

    public void SetSortButtonText(string text)
    {
        sortButtonText.text = text;
    }

    public void SetAscendingButtonSprite(int ascending)
    {
        ascendingButton.image.sprite = ascendingSprite[ascending];
    }
}
