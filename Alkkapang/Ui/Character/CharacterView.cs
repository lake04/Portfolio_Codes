using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : ViewBase
{
    [SerializeField] private CharacterUiData[] characterUiDatas;
    [SerializeField] private GameObject characterItemObject;
    [SerializeField] private Transform  ownedharacterListGameObject;
    [SerializeField] private Transform  notOwnedCharacterListGameObject;

    public void Render(List<CharacterItemViewData> list)
    {
        Clear();

        foreach (var data in list)
        {
            InstantiateCharacterUi(data);
        }
    }

    private void InstantiateCharacterUi(CharacterItemViewData data)
    {
        GameObject characterCard = Instantiate(characterItemObject);

        if(data.isOwned)
        {
            characterCard.transform.parent = ownedharacterListGameObject;
        }
        else
        {
            characterCard.transform.parent = notOwnedCharacterListGameObject;
        }
        if (characterCard.TryGetComponent(out CharacterInfoUi characterUi))
        {
            CharacterUiData uiData = characterUiDatas[data.masterData.characterId];
            characterUi.SetData(data,uiData);
        }
    }

    private void Clear()
    {
        for (int i = ownedharacterListGameObject.childCount - 1; i >= 0; i--)
        {
            Destroy(ownedharacterListGameObject.GetChild(i).gameObject);
        }

        for (int i = notOwnedCharacterListGameObject.childCount - 1; i >= 0; i--)
        {
            Destroy(notOwnedCharacterListGameObject.GetChild(i).gameObject);
        }
    }
}
