using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName="MMORPG Brain/Brains/Monster", order=999)]
public class MonsterBrain : CommonBrain
{
    [Header("Movement")]
    [Range(0, 1)] public float moveProbability = 0.1f; // chance per second
    public float moveDistance = 10;
    public float followDistance = 20;

    [Range(0.1f, 1)] public float attackToMoveRangeRatio = 0.8f; // move as close as 0.8 * attackRange

    // events
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
    string UpdateServer_IDLE(Monster monster)
    {

    }

    string UpdateServer_MOVING(Monster monster)
    {

    }

    string UpdateServer_DEAD(Monster monster)
    {
        // events sorted by priority
        if (EventRespawnTimeElapsed(monster))
        {
            
        }
    }

    public override string UpdateServer(Entity entity)
    {
        Monster monster = (Monster)entity;

        if (monster.state == "IDLE")    return UpdateServer_IDLE(monster);
        if (monster.state == "MOVING")  return UpdateServer_MOVING(monster);
        if (monster.state == "DEAD")    return UpdateServer_DEAD(monster);

        Debug.LogError("invalid state:" + monster.state);
        return "IDLE";

    }

    public override void UpdateClient(Entity entity) { }

    // Debug
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
