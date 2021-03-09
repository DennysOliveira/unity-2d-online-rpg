using UnityEngine;
using Mirror;

public abstract class PlayerAttribute : NetworkBehaviour
{
    [Header("Components")]
    public Level level;
    public Health health;


    [Header("Attribute")]
    [SyncVar] public int value;
    public static int SpendablePerLevel = 2;

    // cache attribute components
    PlayerAttribute[] _attributes;
    PlayerAttribute[] attributes =>
        _attributes ?? (_attributes = GetComponents<PlayerAttribute>());


    public int TotalPointsSpent()
    {
        int spent = 0;
        foreach(PlayerAttribute attribute in attributes)
            spent += attribute.value;
        return spent;
    }

    public int PointsSpendable()
    {
        return (level.current * SpendablePerLevel) - TotalPointsSpent();
    }

    [Command]
    public void CmdIncrease()
    {
        // validate
        if (health.current > 0 && PointsSpendable() > 0)
            ++value;
    }

}
