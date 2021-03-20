using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
[DisallowMultipleComponent]
public class NetworkNavMeshAgent : NetworkBehaviour
{
    public NavMeshAgent agent; // assigned in Inspector
    Vector3 requiredVelocity;
}