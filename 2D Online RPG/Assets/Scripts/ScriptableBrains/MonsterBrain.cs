using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName="MMORPG Brain/Brains/Monster", order=999)]
public class MonsterBrain : CommonBrain
{
    // |- Variable Definitions ->
    [Header("Movement")]
    [Range(0, 1)] public float moveProbability = 0.1f; // chance per second
    public float moveDistance = 10;
    public float followDistance = 20;
    [Range(0.1f, 1)] public float attackToMoveRangeRatio = 0.8f; // move as close as 0.8 * attackRange


    // |- Event Declarations ->
    public bool EventDeathTimeElapsed(Monster monster) =>
        monster.state == "DEAD" && NetworkTime.time >= monster.deathTimeEnd;

    public bool EventMoveRandomly(Monster monster) =>
        Random.value <= moveProbability * Time.deltaTime;

    public bool EventRespawnTimeElapsed(Monster monster) =>
        monster.state == "DEAD" && monster.respawn && NetworkTime.time >= monster.respawnTimeEnd;

    public bool EventTargetTooFarToFollow(Monster monster) =>
        monster.target != null &&
        Vector3.Distance(monster.startPosition, Utils.ClosestPoint(monster.target, monster.transform.position)) > followDistance;


    // |- States ->
    string ServerUpdateStateFrom_IDLE(Monster monster)
    {
        if (EventDied(monster)) { return "DEAD"; }
        if (EventTargetDied(monster)) { monster.target = null; return "IDLE"; }
        if (EventTargetTooFarToFollow(monster)) { monster.target = null; monster.transform.position = monster.startPosition; return "MOVING"; }
        if (EventAggro(monster)) { return "IDLE"; }
        if (EventMoveRandomly(monster))
        {
            Vector2 circle2D = Random.insideUnitCircle * moveDistance;
            //monster.movement.Navigate(monster.startPosition + new Vector3(circle2D.x, circle2D.y, monster.transform.position.z));
            return "MOVING";
        }
        if (EventDeathTimeElapsed(monster)) {} // don't care
        if (EventRespawnTimeElapsed(monster)) {} // don't care
        if (EventMoveEnd(monster)) {} // don't care
        if (EventTargetDisappeared(monster)) {} // don't care

        return "IDLE";
    }

    string ServerUpdateStateFrom_MOVING(Monster monster)
    {
        if (EventDied(monster))
        {
            //monster.movement.Reset();
            return "DEAD";
        }
        if (EventMoveEnd(monster))
        {
            return "IDLE";
        }
        if (EventTargetDied(monster))
        {
            monster.target = null;
            return "IDLE";
        }
        if (EventTargetTooFarToFollow(monster))
        {
            monster.target = null;
            //monster.movement.navigate(monster.startPosition, 0);
            return "MOVING";
        }
        if (EventAggro(monster))
        {
            return "IDLE";
        }
        if (EventDeathTimeElapsed(monster)) {} // don't care
        if (EventRespawnTimeElapsed(monster)) {} // don't care
        if (EventTargetDisappeared(monster)) {} // don't care
        if (EventMoveRandomly(monster)) {} // don't care

        return "MOVING";
    }

    string ServerUpdateStateFrom_DEAD(Monster monster)
    {
        if (EventRespawnTimeElapsed(monster))
        {
            // respawn
            monster.Show();
            monster.transform.position = monster.startPosition;
            monster.Revive();
            return "IDLE";
        }
        if (EventDeathTimeElapsed(monster)) 
        {
            if(monster.respawn) monster.Hide();
            else NetworkServer.Destroy(monster.gameObject);
            return "DEAD";
        }
        if (EventMoveEnd(monster)) {} // don't care
        if (EventTargetDisappeared(monster)) {} // don't care
        if (EventTargetDied(monster)) {} // don't care
        if (EventTargetTooFarToFollow(monster)) {} // don't care
        if (EventAggro(monster)) {} // don't care
        if (EventMoveRandomly(monster)) {} // don't care
        if (EventDied(monster)) {} // don't care, of course we are dead

        return "DEAD"; // nothing interesting happened
    }

    public override string UpdateServer(Entity entity)
    {
        Monster monster = (Monster)entity;

        if (monster.state == "IDLE")    return ServerUpdateStateFrom_IDLE(monster);
        if (monster.state == "MOVING")  return ServerUpdateStateFrom_MOVING(monster);
        if (monster.state == "DEAD")    return ServerUpdateStateFrom_DEAD(monster);

        Debug.LogError("invalid state:" + monster.state);
        return "IDLE";

    }

    public override void UpdateClient(Entity entity) { }


    // |- Debugging ->
    public override void DrawGizmos(Entity entity)
    {
        Monster monster = (Monster)entity;

        // draw the movement area (around 'start' if game running,
        // or around current position if still editing)
        Vector3 startHelp = Application.isPlaying ? monster.startPosition : monster.transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startHelp, moveDistance);

        // draw the follow dist
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(startHelp, followDistance);
    }
}
