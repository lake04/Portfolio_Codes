using Spine.Unity;
using UnityEngine;

public class CharacterDetailModel : ModelBase
{
    public CharacterMasterData characterData;

    public async void SetSelectState()
    {
        bool isSuccess = await CharacterDataManager.Instance.EquipCharacterByIdAsync(characterData.characterId);

        if (isSuccess)
        {
            Debug.Log($"캐릭터 장착 완료 : {characterData.name}");

            if (UiManager.instance != null && UiManager.instance.homeSkeletonhGraphic != null)
            {
                CharacterDataManager.Instance.ChangeCharacterHomeModel(
                    UiManager.instance.homeSkeletonhGraphic,
                    characterData.characterId
                );
            }
        }
        else
        {
            Debug.LogError($"캐릭터 장착 실패");
        }
    }
}
