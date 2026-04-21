using System.Collections.Generic;

public class CharacterModel : ModelBase
{
    public List<CharacterItemViewData> CharacterList { get; private set; } = new();

    public void SetCharacterList(List<CharacterItemViewData> list)
    {
        CharacterList = list;
    }
}