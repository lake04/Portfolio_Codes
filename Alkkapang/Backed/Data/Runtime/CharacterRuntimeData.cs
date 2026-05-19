using System;
using UnityEngine;

[Serializable]
public class CharacterRuntimeData
{
    private const float DefaultWeight = CharacterStatNormalizer.DefaultStat;
    private const float DefaultSpeed = CharacterStatNormalizer.DefaultStat;
    private const float DefaultDefense = 0f;
    private const float DefaultPower = CharacterStatNormalizer.DefaultStat;
    private const float DefaultHandling = CharacterStatNormalizer.DefaultStat;
    private const float DefaultLinearDamping = 1.5f;
    private const float DefaultAngularDamping = 1f;

    public int CharacterId;
    public string CharacterKey;
    public string DisplayName;
    public int Rarity;
    public string Role;
    public bool IsActive;
    public string VisualKey;
    public string AbilityId;
    public CharacterStatBlock StatBlock;

    public float LaunchSpeedMultiplier;
    public float ImpactPower;
    public float LinearDamping;
    public float AngularDamping;
    public float StopThreshold;

    public CharacterRuntimeData(
        int characterId,
        string characterKey,
        string displayName,
        int rarity,
        string role,
        bool isActive,
        string abilityId,
        CharacterStatBlock statBlock,
        float baseStopThreshold = 1f)
    {
        CharacterId = characterId;
        CharacterKey = characterKey;
        DisplayName = displayName;
        Rarity = rarity;
        Role = role;
        IsActive = isActive;
        AbilityId = abilityId;
        StatBlock = statBlock;

        RecalculateDerivedStats(baseStopThreshold);
    }

    public void RecalculateDerivedStats(float baseStopThreshold = 1f)
    {
        StatBlock.Weight = CharacterStatNormalizer.ClampStat(StatBlock.Weight > 0f ? StatBlock.Weight : DefaultWeight);
        StatBlock.Speed = CharacterStatNormalizer.ClampStat(StatBlock.Speed > 0f ? StatBlock.Speed : DefaultSpeed);
        StatBlock.Defense = CharacterStatNormalizer.ClampDefense(StatBlock.Defense >= 0f ? StatBlock.Defense : DefaultDefense);
        StatBlock.Power = CharacterStatNormalizer.ClampStat(StatBlock.Power > 0f ? StatBlock.Power : DefaultPower);
        StatBlock.Handling = CharacterStatNormalizer.ClampStat(StatBlock.Handling > 0f ? StatBlock.Handling : DefaultHandling);

        float normalizedSpeed = CharacterStatNormalizer.NormalizeSpeed(StatBlock.Speed);
        float normalizedPower = CharacterStatNormalizer.NormalizePower(StatBlock.Power);
        float normalizedHandling = CharacterStatNormalizer.NormalizeHandling(StatBlock.Handling);

        LaunchSpeedMultiplier = normalizedSpeed;
        ImpactPower = normalizedPower;
        LinearDamping = DefaultLinearDamping;
        AngularDamping = DefaultAngularDamping;
        StopThreshold = Mathf.Max(0.01f, baseStopThreshold) * normalizedHandling;
    }
}
