using UnityEngine;
using Mirror;

public abstract class CommonBrain : ScriptableBrain
{
    public bool EventAggro(Entity entity) =>
        entity.target != null && entity.target.health.current > 0;

    public bool EventDied(Entity entity) =>
        entity.health.current == 0;



    public bool EventMoveEnd(Entity entity) => entity.state != "MOVING";
    public bool EventMoveStart(Entity entity) => entity.state == "MOVING";

    // Skill Request event

    // Skill Finished event

    public bool EventTargetDied(Entity entity) =>
        entity.target != null && entity.target.health.current == 0;
    
    public bool EventTargetDisappeared(Entity entity) =>
        entity.target == null;
    
}
