using System;
using BackEnd;
using LitJson;

[Serializable]
public class UserCharacterData
{
    public int characterId;
    public bool isEquipped;

    public UserCharacterData() { }

    public UserCharacterData(int characterId, bool isEquipped = false)
    {
        this.characterId = characterId;
        this.isEquipped = isEquipped;
    }

    public UserCharacterData(OwnedUserCharacter data)
    {
        characterId = data.Id;
        isEquipped = data.IsEquipped;
    }
}