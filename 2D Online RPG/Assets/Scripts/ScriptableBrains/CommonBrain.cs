using UnityEngine;
using Mirror;

public abstract class CommonBrain : ScriptableBrain
{
    public bool EventAggro(Entity entity) =>
        entity.target != null && entity.target.health.current > 0;

    public bool EventDied(Entity entity) =>
        entity.health.current == 0;

    // Moving start event

    // Moving end event

    // Skill Request event

    // Skill Finished event

    public bool EventTargetDied(Entity entity) =>
        entity.target != null && entity.target.health.current == 0;
    
    public bool EventTargetDisappeared(Entity entity) =>
        entity.target == null;
    
}
