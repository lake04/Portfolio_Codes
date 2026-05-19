using UnityEngine;

public class BackEndCharacterDataRepository : ICharacterDataRepository
{
    public bool TryGetCharacterRuntimeData(int characterId, out CharacterRuntimeData runtimeData)
    {
        runtimeData = null;

        CharacterDataManager manager = CharacterDataManager.Instance;
        if (manager == null)
        {
            Debug.LogWarning("[BackEndCharacterDataRepository] CharacterDataManager is missing. Runtime character data cannot be loaded.");
            return false;
        }

        if (!manager.IsDataReady)
        {
            Debug.LogWarning($"[BackEndCharacterDataRepository] Backend character data is not ready yet. CharacterId:{characterId}");
            return false;
        }

        CharacterMasterData masterData = manager.GetMaster(characterId);
        if (masterData == null)
        {
            Debug.LogError($"[BackEndCharacterDataRepository] Character master not found in backend data. CharacterId:{characterId}");
            return false;
        }

        if (!HasValidStats(masterData))
        {
            Debug.LogWarning(
                $"[BackEndCharacterDataRepository] Invalid backend stats. " +
                $"CharacterId:{masterData.characterId}, Name:{masterData.name}, " +
                $"Weight:{masterData.weight}, Speed:{masterData.speed}, Defense:{masterData.defense}, " +
                $"Power:{masterData.power}, Handling:{masterData.handling}. " +
                "Backend data ignored. Runtime character data will not be applied.");
            return false;
        }

        runtimeData = CharacterDataMapper.FromBackEndMaster(masterData);
        Debug.Log(
            $"[BackEndCharacterDataRepository] Backend stats applied. " +
            $"CharacterId:{masterData.characterId}, Key:{masterData.characterKey}, Name:{masterData.name}, AbilityId:{masterData.abilityId}, " +
            $"Weight:{masterData.weight}, Speed:{masterData.speed}, Defense:{masterData.defense}, " +
            $"Power:{masterData.power}, Handling:{masterData.handling}");
        return runtimeData != null;
    }

    private static bool HasValidStats(CharacterMasterData data)
    {
        return CharacterStatNormalizer.IsValidStat(data.weight)
            && CharacterStatNormalizer.IsValidStat(data.speed)
            && CharacterStatNormalizer.IsValidDefense(data.defense)
            && CharacterStatNormalizer.IsValidStat(data.power)
            && CharacterStatNormalizer.IsValidStat(data.handling);
    }
}
