public static class CharacterRuntimeDataProvider
{
    private static readonly ICharacterDataRepository BackEndRepository = new BackEndCharacterDataRepository();
    private static readonly LocalCharacterDataRepository LocalRepository = new LocalCharacterDataRepository();

    public static bool TryGetRuntimeData(int characterId, out CharacterRuntimeData runtimeData)
    {
        return BackEndRepository.TryGetCharacterRuntimeData(characterId, out runtimeData);
    }

    public static bool TryGetFallbackVisualData(int characterId, out StoneData stoneData)
    {
        return LocalRepository.TryGetStoneData(characterId, out stoneData);
    }
}
