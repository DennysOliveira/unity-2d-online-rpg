using UnityEngine;
using Mirror;

public class PlayerExperience : Experience
{
    [Header("Components")]
    // chat
    // party

    [Header("Death")]
    public string deathMsg = "You died and lost experience.";

    [Server]
    public override void OnDeath()
    {
        base.OnDeath();

        // chat.AlertMsg(deathMsg);
    }


    // events
    [Server]
    public void OnKilledEnemy(Entity victim)
    {
        if(victim is Monster monster)
        {
            current += monster.rewardExperience;
        }
    }
}
