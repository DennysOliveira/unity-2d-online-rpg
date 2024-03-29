﻿using UnityEngine;
using Mirror;

public abstract class Movement : NetworkBehaviour
{
    // velocity is useful for animation etc.
    public abstract Vector3 GetVelocity();

    // currently moving? 
    public abstract bool IsMoving();

    // .speed lives in Entity and depends on level, skills, equip, etc.
    public abstract void SetSpeed(float speed);

    public abstract void LookAt2D(Vector2 position);

    // Reset all movement. Stop and stand.
    public abstract void Reset();

    // Warp like transform.position
    public abstract void Warp(Vector3 destination);

    public abstract bool CanNavigate();

    // navigate along a path to a destination
    public abstract void Navigate(Vector3 destination, float stoppingDistance);


    public abstract bool IsValidSpawnPoint(Vector3 position);

    // Check if the next destination is valid
    public abstract Vector3 NearestValidDestination(Vector3 destination);

    // Should auto-look at the target during combat?
    public abstract bool DoCombatLookAt();

}
