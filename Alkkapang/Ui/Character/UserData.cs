using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public List<int> ownedCharacterIds = new();

    public int equippedCharacterId = -1;
}