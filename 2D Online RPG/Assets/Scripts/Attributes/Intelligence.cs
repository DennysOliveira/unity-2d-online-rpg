using System;
using UnityEngine;


[DisallowMultipleComponent]
public class Intelligence : PlayerAttribute, IManaBonus
{
    // 1 point means 10 mana points
    public int manaBonusPerPoint = 10;

    public int GetManaBonus(int baseMana) =>
        Convert.ToInt32(baseMana + (value * manaBonusPerPoint));

    public int GetManaRecoveryBonus() => 
        Convert.ToInt32((value * 0.1f));
}
