using UnityEngine;
using Mirror;

public class Monster : Entity
{
    [Header("Components")]

    [Header("Experience Reward")]
    public long rewardExperience = 1;
    // public long rewardSkillExperience = 2;

    
    [Header("Respawn")]
    public float deathTime = 30f;
    [HideInInspector] public double deathTimeEnd;
    public bool respawn = true;
    public float respawnTime = 10f;
    [HideInInspector] public double respawnTimeEnd;


    // save the start position for random movements
    [HideInInspector] public Vector3 startPosition;


    // NetworkBehaviour ->
    protected override void Start()
    {
        base.Start();

        // remember start position in case we need to respawn later
        startPosition = transform.position;
    }

    void LateUpdate() {
        if (isClient)
        {
            animator.SetBool("MOVING", state == "MOVING");
        }
    }

    
    // Aggro ->
    [ServerCallback]
    public override void OnAggro(Entity entity)
    {
        base.OnAggro(entity);

        if(CanAttack(entity))
        {

            if (target == null)
            {
                target = entity;
            }
            else if (entity != target) // no need to check dist for same target
            {
                float oldDistance = Vector3.Distance(transform.position, target.transform.position);
                float newDistance = Vector3.Distance(transform.position, entity.transform.position);
                if (newDistance < oldDistance * 0.8) target = entity;
            }
        }
    }

    
    // |- Death -|-> 
    [Server]
    public override void OnDeath()
    {
        // take care of entity stuff
        base.OnDeath();

        // set death and respawn end times. we set both of them now to make sure
        // that everything works fine even if a monster isn't updated for a
        // while. so as soon as it's updated again, the death/respawn will
        // happen immediately if current time > end time.
        deathTimeEnd = NetworkTime.time + deathTime;
        respawnTimeEnd = deathTimeEnd + respawnTime; // after death time ended
    }

    // |- Entity -> Player.CanAttack -|
    public override bool CanAttack(Entity entity)
    {
        return (base.CanAttack(entity) && (entity is Player));
    }

    

}
