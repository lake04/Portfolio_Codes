using System;

[Serializable]
public struct CharacterStatBlock
{
    public float Weight;
    public float Speed;
    public float Defense;
    public float Power;
    public float Handling;

    public CharacterStatBlock(float weight, float speed, float defense, float power, float handling)
    {
        Weight = weight;
        Speed = speed;
        Defense = defense;
        Power = power;
        Handling = handling;
    }
}
