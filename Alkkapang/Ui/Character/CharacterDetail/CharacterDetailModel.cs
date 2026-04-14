using UnityEngine;

public class CharacterDetailModel : ModelBase
{
   

    public CharacterMasterData characterData;


    

    public void SetSelectState()
    {
        CharacterDataManager.Instance.EquipCharacterById(characterData.characterId);
        
    }
}
