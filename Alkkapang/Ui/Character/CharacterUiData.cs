using UnityEngine;

[CreateAssetMenu(fileName = "characterUiData", menuName = "ScriptableObjects/characterUiDatas")]
public class CharacterUiData : ScriptableObject
{
    public Sprite character;
    public Sprite stage;
    public Sprite capsule;
    [TextArea(3, 10)]
    public string desc;
}
