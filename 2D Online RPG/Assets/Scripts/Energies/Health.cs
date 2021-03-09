using UnityEngine;


public interface IHealthBonus
{
    int GetHealthBonus(int baseHealth);
    int GetHealthRecoveryBonus();
}


[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Health : Energy
{
    public Level level;
    public LinearInt baseHealth = new LinearInt{baseValue=100};
    public int baseRecoveryRate = 1;


    // cache components that give bonuses (attributes, inventory, etc.)
    // assigned when needed.
    IHealthBonus[] _bonusComponents;
    IHealthBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IHealthBonus>());


    // Calculates Max Health with base for Current Level + Bonuses
    public override int max
    {
        get
        {
            int bonus = 0;
            int baseForCurrentLevel = baseHealth.Get(level.current);
            foreach (IHealthBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetHealthBonus(baseForCurrentLevel);
            
            return baseForCurrentLevel + bonus;
        }
    }


    // Calculates Recovery Rate with Bonuses
    public override int recoveryRate
    {
        get
        {
            int bonus = 0;
            foreach(IHealthBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetHealthRecoveryBonus();
            
            return baseRecoveryRate + bonus;
        }
    }

}
