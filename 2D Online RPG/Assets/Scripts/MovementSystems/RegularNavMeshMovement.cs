// Regular NavMesh movement for Monsters and Pets.
// -> uses: NetworkNavMeshAgent 
// instead of 
// NetworkNavMeshAgentRubberbanding
using UnityEngine;

[RequireComponent(typeof(NetworkNavMeshAgent))]
[DisallowMultipleComponent]
public class RegularNavMeshMovement : NavMeshMovement
{
    [Header("Components")]
    public NetworkNavMeshAgent networkNavMeshAgent;

    public override void Reset()
    {
        agent.ResetMovement();
    }

    public override void Warp(Vector3 destination)
    {
        // NetworkNavMeshAgent needs to know about Warp. 
        if (isServer)
            networkNavMeshAgent.RpcWarp(destination);

        agent.Warp(destination);
    }
}