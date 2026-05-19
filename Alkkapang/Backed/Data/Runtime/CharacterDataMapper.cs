public static class CharacterDataMapper
{
    private const float LocalDefaultDefense = 0f;
    private const int LocalFootballCharacterId = 2;
    private const string LocalFootballAbilityId = "soccer_flame_shot";

    public static CharacterRuntimeData FromStoneData(int characterId, StoneData data)
    {
        if (data == null)
            return null;

        string displayName = string.IsNullOrEmpty(data.characterName)
            ? $"Character_{characterId}"
            : data.characterName;

        var statBlock = new CharacterStatBlock(
            data.weight,
            data.launchSpeedMultiplier,
            LocalDefaultDefense,
            data.impactPower,
            data.handling);

        return new CharacterRuntimeData(
            characterId,
            string.Empty,
            displayName,
            1,
            string.Empty,
            true,
            GetAbilityId(characterId, data.abilityId),
            statBlock,
            data.baseStopThreshold);
    }

    public static CharacterRuntimeData FromBackEndMaster(CharacterMasterData data)
    {
        if (data == null)
            return null;

        var statBlock = new CharacterStatBlock(
            data.weight,
            data.speed,
            data.defense,
            data.power,
            data.handling);

        return new CharacterRuntimeData(
            data.characterId,
            data.characterKey,
            data.name,
            data.rarity,
            data.role,
            data.isActive,
            GetAbilityId(data.characterId, data.abilityId),
            statBlock);
    }

    public static CharacterRuntimeData FromBackEndMaster(CharacterMaster data)
    {
        if (data == null)
            return null;

        return FromBackEndMaster(new CharacterMasterData(data));
    }

    private static string NormalizeAbilityId(string configuredAbilityId)
    {
        return string.IsNullOrWhiteSpace(configuredAbilityId)
            ? string.Empty
            : configuredAbilityId.Trim();
    }

    private static string GetAbilityId(int characterId, string configuredAbilityId)
    {
        string abilityId = NormalizeAbilityId(configuredAbilityId);
        if (!string.IsNullOrEmpty(abilityId))
            return abilityId;

        return characterId == LocalFootballCharacterId
            ? LocalFootballAbilityId
            : string.Empty;
    }

}
