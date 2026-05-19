using Spine.Unity;

[System.Serializable]
public class CharacterSpineAsset
{
    public int characterId;             
    public SkeletonDataAsset dataAsset; 
    public string defaultSkinName = "default";
    public string defaultAnimationName = "idle";
    public int direction;
}