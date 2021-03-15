using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName="MMORPG Brain/Brains/Player", order=999)]
public class PlayerBrain : CommonBrain
{
    // Events ->
    public bool EventCancelAction(Player player)
    {
        bool result = player.cancelActionRequested;
        player.cancelActionRequested = false; // reset
        return result;
    }

    public bool EventRespawn(Player player)
    {
        bool result = player.respawnRequested;
        player.respawnRequested = false; // reset
        return result;
    }

    // ||-|---------->
    // ||-|- States ->
    string UpdateServer_IDLE(Player player)
    {
        if (EventDied(player))
        {
            // we died.
            return "DEAD";
        }

        if (EventCancelAction(player))
        {
            // the only thing that we can cancel is the target
            player.target = null;
            return "IDLE";
        }

        if (EventMoveStart(player))
        {
            return "MOVING";
        }
    
        return "IDLE";
    }

    string UpdateServer_MOVING(Player player)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventDied(player))
        {
            // we died.
            return "DEAD";
        }
        
        if (EventMoveEnd(player))
        {
            // finished moving. do whatever we did before.
            return "IDLE";
        }
        
        if (EventCancelAction(player))
        {
            // cancel casting (if any) and stop moving
            //player.skills.CancelCast();
            //player.movement.Reset(); <- done locally. doing it here would reset localplayer to the slightly behind server position otherwise
            return "IDLE";
        }
    
        return "MOVING";
    }

    string UpdateServer_DEAD(Player player)
    {
        // events sorted by priority (e.g. target doesn't matter if we died)
        if (EventRespawn(player))
        {
            // revive to closest spawn, with 50% health, then go to idle
            Vector2 startPosition = new Vector2(0, 0);
            player.transform.position = startPosition; // recommended over transform.position
            player.Revive(0.5f);
            return "IDLE";
        }

        return "DEAD";

    }

    public override string UpdateServer(Entity entity)
    {
        Player player = (Player)entity;

        if (player.state == "IDLE")     return UpdateServer_IDLE(player);
        if (player.state == "MOVING")   return UpdateServer_MOVING(player);
        if (player.state == "DEAD")     return UpdateServer_DEAD(player);

        return "IDLE";
    }

    public override void UpdateClient(Entity entity)
    {
        Player player = (Player)entity;

        if(player.state == "IDLE" || player.state == "MOVING")
        {
            if (Input.GetKeyDown(player.cancelActionKey))
            {
                // player.movement.Reset(); // reset locally because we use rubberband movement
                player.CmdCancelAction();
            }
        }
        else if (player.state == "DEAD") {}
        else Debug.LogError("invalid state:" + player.state);
    }

}
