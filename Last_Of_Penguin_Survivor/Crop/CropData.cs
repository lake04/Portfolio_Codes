using Island;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Crop", menuName = "Add Crop/CropData")]
public class CropData : ScriptableObject
{
    public string cropsName;
    public string cropsId;

    public List<BlockConstants>  cultivableList;
    public float[] growthSpeed;
    public InventoryItem dropItem;
    public Vector3[] cropsSize;
}
