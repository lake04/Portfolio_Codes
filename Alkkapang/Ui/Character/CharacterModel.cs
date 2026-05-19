using System;
using System.Collections.Generic;

public enum CharacterSortType
{
    Name,
    Rarity,
    //Level,
    //Acquired
}

public class CharacterModel : ModelBase
{
    public List<CharacterItemViewData> CharacterList { get; private set; } = new();

    private CharacterSortType currentSortType = CharacterSortType.Name;

    public CharacterSortType CurrentSortType => currentSortType;

    private int sortAscending = 1;

    public int SortAscending => sortAscending;

    public void SetCharacterList(List<CharacterItemViewData> list)
    {
        CharacterList = list;
    }

    public void SortCharacterList()
    {
        int enumCount = Enum.GetValues(typeof(CharacterSortType)).Length;
        //int enumCount = 2;
        currentSortType = (CharacterSortType)(((int)currentSortType + 1) % enumCount);

        switch (currentSortType)
        {
            case CharacterSortType.Name:
                SortByName();
                break;
            case CharacterSortType.Rarity:
                SortByRarity();
                break;
            //case CharacterSortType.Level:
            //    break;
            //case CharacterSortType.Acquired:
            //    break;
        }
    }

    public void Ascending()
    {
        if(sortAscending == 1)
        {
            sortAscending = 0;
        }
        else
        {
            sortAscending = 1;
        }
        SortCharacterList();
    }

    public void SortByName()
    {
        if (sortAscending == 1)
        {
            CharacterList.Sort((a, b) => a.masterData.name.CompareTo(b.masterData.name));
        }
        else
        {
            CharacterList.Sort((a, b) => b.masterData.name.CompareTo(a.masterData.name));
        }
    }

    public void SortByRarity()
    {
        if (sortAscending == 1)
        {
            CharacterList.Sort((a, b) => a.masterData.rarity.CompareTo(b.masterData.rarity));
        }
        else
        {
            CharacterList.Sort((a, b) => b.masterData.rarity.CompareTo(a.masterData.rarity));
        }
    }
}