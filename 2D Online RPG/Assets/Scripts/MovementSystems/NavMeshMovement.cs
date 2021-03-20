using UnityEngine;
using UnityEngine.AI;
using Mirror;

// Require NetworkNavMeshAgent => Players use a different sync method, can't require it.
[RequireComponent(typeof(NavMeshAgent))]
[DisallowMultipleComponent]
public abstract class NavMeshMovement : Movement
{
    [Header("Components")]
    public NavMeshAgent agent;

    public override Vector3 GetVelocity() => 
        agent.velocity;

    public override bool IsMoving() =>
        agent.pathPending ||
        agent.remainingDistance > agent.stoppingDistance ||
        agent.velocity != Vector3.zero;

    public override void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public override void LookAt2D(Vector2 position)
    {
        Debug.Log("LookAt2D> looking at Vector2 position.");
    }

    public override bool CanNavigate()
    {
        return true;
    }

    public override void Navigate(Vector3 destination, float stoppingDistance)
    {
        agent.stoppingDistance = stoppingDistance;
        agent.destination = destination;
    }

    // when spawning we need to know if the last saved position is still valid
    // for this type of movement.

    public override bool IsValidSpawnPoint(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit _, 0.1f, NavMesh.AllAreas);
    }

    public override Vector3 NearestValidDestination(Vector3 destination)
    {
        return agent.NearestValidDestination(destination);
    }

    public override bool DoCombatLookAt()
    {
        return true;
    }

    [Server]
    public void OnDeath()
    {
        Reset();
    }

}
