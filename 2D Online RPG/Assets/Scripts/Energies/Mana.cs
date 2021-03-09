using UnityEngine;


public interface IManaBonus
{
    int GetManaBonus(int baseMana);
    int GetManaRecoveryBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Mana : Energy
{
    public Level level;
    public LinearInt baseMana = new LinearInt{ baseValue=10 };
    public int baseRecoveryRate = 1;

    
    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IManaBonus[] _bonusComponents;
    IManaBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IManaBonus>());

    
    // calculate max
    public override int max
    {
        get
        {
            int bonus = 0;
            int baseForCurrentLevel = baseMana.Get(level.current);
            foreach (IManaBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetManaBonus(baseForCurrentLevel);

            return baseForCurrentLevel + bonus;
        }
    }


    public override int recoveryRate
    {
        get
        {
            int bonus = 0;
            foreach (IManaBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetManaRecoveryBonus();

            return baseRecoveryRate + bonus;
        }
    }
}
