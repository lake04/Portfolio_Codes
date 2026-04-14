using Island;
using System.Collections.Generic;
using UnityEngine;

public enum DisasterType
{
    Weather,
    Monster,
    Earth
}


[CreateAssetMenu(fileName = "NewDisasterData", menuName = "GameData/DisasterData")]
public class DisasterData : ScriptableObject
{
    [Header("기본 정보")]
    public string Name;

    [Range(1, 3)]
    public int Level;

    public DisasterType Type;

    public string Island;

    public float Duration;

    public TemperatureType TemperatureChange;

    public List<string> DebuffList = new List<string>();

    public List<string> DamageEffects = new List<string>();

    public List<string> SpawnEntities = new List<string>();

    public List<string> BlockInteraction = new List<string>();
}
