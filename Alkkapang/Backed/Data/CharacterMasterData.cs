using System;
using LitJson;

[Serializable]
public class CharacterMasterData
{
    public int characterId;
    public string characterKey;
    public string name;
    public int weight;
    public int speed;
    public int defense;
    public int power;
    public int handling;
    public string role;
    public string abilityId;
    public string abilityDesc;
    public int rarity;
    public bool isActive;
    public int sortOrder;

    public CharacterMasterData() { }

    public CharacterMasterData(CharacterMaster data)
    {
        characterId = data.CharacterId;
        characterKey = data.CharacterKey;
        name = data.Name;
        weight = data.Weight;
        speed = data.Speed;
        defense = data.Defense;
        power = data.Power;
        handling = data.Handling;
        role = data.Role;
        abilityId = data.AbilityId;
        abilityDesc = data.AbilityDesc;
        rarity = data.Rarity;
        isActive = data.IsActive;
        sortOrder = data.SortOrder;
    }
}