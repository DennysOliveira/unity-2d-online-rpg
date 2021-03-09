using UnityEngine;

public abstract class ScriptableBrain : ScriptableObject
{
    // updates server state machine, returns next state
    public abstract string UpdateServer(Entity entity);


    // updates client state machine
    public abstract void UpdateClient(Entity entity);

    // DrawGizmos can be used to display debug information
    // (can't name it "On"DrawGizmos otherwise Unity complains about parameters)
    public virtual void DrawGizmos(Entity entity) {}
}
