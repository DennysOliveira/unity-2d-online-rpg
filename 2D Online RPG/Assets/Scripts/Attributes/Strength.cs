// Strength Attribute
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Strength : PlayerAttribute, IHealthBonus
{
    public float healthBonusPercentPerPoint = 5;

    public int GetHealthBonus(int baseHealth) =>
        Convert.ToInt32(baseHealth + (value * healthBonusPercentPerPoint));

    public int GetHealthRecoveryBonus() => 0;

}
