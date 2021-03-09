using System;
using UnityEngine;

[Serializable]
public struct LinearInt
{
    public int baseValue;
    public int bonusPerLevel;
    public int Get(int level) => bonusPerLevel * (level - 1) + baseValue;
    //              level 10             10 * (level 10 - 1) + base(100)
    //                              10 * 9 + 100;
}

[Serializable]
public struct LinearFloat
{
    public float baseValue;
    public float bonusPerLevel;
    public float Get(int level) => bonusPerLevel * (level - 1) + baseValue;
}

[Serializable]
public struct ExponentialInt
{
    public int multiplier;
    public float baseValue;

    public int Get(int level) => 
        Convert.ToInt32(multiplier * Mathf.Pow(baseValue, (level -1)));
}

[Serializable]
public struct ExponentialLong
{
    public long multiplier;
    public float baseValue;

    public long Get(int level) => 
        Convert.ToInt64(multiplier * Mathf.Pow(baseValue, (level -1)));
}

[Serializable]
public struct ExponentialFloat
{
    public float multiplier;
    public float baseValue;

    public float Get(int level) => 
        multiplier * Mathf.Pow(baseValue, (level -1));
}