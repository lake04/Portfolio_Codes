using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public string inDate = string.Empty;
    public List<int> ownedCharacterIds = new();

    public int equippedCharacterId = -1;
}