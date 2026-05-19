public interface ICharacterDataRepository
{
    bool TryGetCharacterRuntimeData(int characterId, out CharacterRuntimeData runtimeData);
}
