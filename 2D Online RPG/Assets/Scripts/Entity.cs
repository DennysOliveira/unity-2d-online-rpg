using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Mirror;
using TMPro;

[Serializable] public class UnityEventEntity : UnityEvent<Entity> {}
[Serializable] public class UnityEventEntityInt : UnityEvent<Entity, int> {}


[RequireComponent(typeof(Level))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Mana))]
[RequireComponent(typeof(Combat))] // TO-DO
[RequireComponent(typeof(Skills))] // TO-DO
[RequireComponent(typeof(NetworkProximityGridChecker))] // TO-DO
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))] // kinematic, only needed for OnTrigger (?)
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public abstract partial class Entity : NetworkBehaviour
{
    [Header("Components")]
    public Level level;
    public Health health;
    public Mana mana;
    public Animator animator;
    public AudioSource audioSource;

    // finite state machine
    // -> state is only writable by entity class to avoid confusion
    [Header("Brain")]
    public ScriptableBrain brain;
    [SyncVar, SerializeField] string _state = "IDLE";
    public string state => _state;


    // it's useful to know an entity's last combat time (did/was attacked)
    // e.g. to prevent logging out for x seconds after combat
    [SyncVar] public double lastCombatTime;


    [Header("Target")]
    [SyncVar, HideInInspector] public Entity target;


    [Header("Speed")]
    [SerializeField] protected LinearFloat _speed = new LinearFloat{ baseValue = 2 };
    public virtual float speed
    {
        get
        {
            float passiveBonus = 0;
            float buffBonus = 0;
            float equipmentBonus = 0;

            return _speed.Get(0) + passiveBonus + buffBonus + equipmentBonus;
        }
    }

    [Header("Events")]
    public UnityEventEntity onAggro;
    public UnityEvent onLeftClick;  // called when left-clicking an unity
    public UnityEvent onRightClick; // called when right-clicking an unity


    // safe zone flag
    [HideInInspector] public bool inSafeZone;


    // Network Behaviour  =>
    public override void OnStartServer()
    {
        // dead if spawned without health
        if (health.current == 0)
            _state = "DEAD";
    }


    public virtual void Start()
    {
        // disable animator on the server (Significative Performance Boost)
        if (!isClient) animator.enabled = false;
    }

    [Server]
    public virtual bool IsWorthUpdating()
    {
        return netIdentity.observers.Count > 0 || 
               IsHidden();
    }

    // Entity logic will be implemented using a State Machine
    // -> We react to every state and to every event for correctness
    // -> We keep it functional for simplicity
    // LateUpdate can still be used for Updates that should happen in any case.


    void Update()
    {
        // always apply speed to movement system
        // (if any. mounts don't have one)
        // if (movement != null)
        //     movement.SetSpeed(speed);

        // always update all the objects that the client sees
        if (isClient)
            brain.UpdateClient(this);

        
        // on server, only update if worth updating
        if (isServer && IsWorthUpdating())
        {
            if (target != null && target.IsHidden())
                target = null;
            _state = brain.UpdateServer(this);
        }
    }

    // OnDrawGizmos only happens while the Script is not collapsed
    public virtual void OnDrawGizmos()
    {
        if (brain != null)
            brain.DrawGizmos(this);
    }


    // Visibility ->
    // Hides an entity
    [Server]
    public void Hide()
    {
        proxchecker.forceHidden = true;
    }

    [Server]
    public void Show()
    {
        proxchecker.forceHidden = false;
    }


    // is the entity currently hidden?
    // note: usually the server is the only one who uses forceHidden, the
    //       client usually doesn't know about it and simply doesn't see the
    //       GameObject.
    public bool IsHidden() => proxchecker.forceHidden;

    public float VisRange() => NetworkProximityGridChecker.visRange;


    // Revival ->
    // Revives with 100% of HP
    [Server]
    public void Revive(float healthPercentage = 1)
    {
        health.current = Mathf.RoundToInt(health.max * healthPercentage);
    }

    
    // Aggro ->
    // This function is called when pulling aggro
    public virtual void OnAgrro(Entity entity)
    {
        onAggro.Invoke(entity);
    }

    // Check if entity can attack another entity.
    public virtual bool CanAttack(Entity entity)
    {
        return health.current > 0 && !inSafeZone && !entity.inSafeZone;
    }

    // death ///////////////////////////////////////////////////////////////////
    // universal OnDeath function that takes care of all the Entity stuff.
    // should be called by inheriting classes' finite state machine on death.
    [Server]
    public virtual void OnDeath()
    {
        // clear target
        target = null;
    }

    // Selection and Interaction
    // uses Unity OnMouseDown function.
    void OnMouseDown()
    {
        if(Player.localPlayer != null) //!Utils.IsCursorOverInterface() &&  //(Player.localPlayer.state == "IDLE" || "MOVING" || "CASTING" || "STUNNED"))
        {
            // Set Indicator in any case
            // Player.localPlayer.indicator.SetViaParent(transform);

            // clicked for the first time: SELECT
            if(Player.localPlayer.target != this)
            {
                Player.localPlayer.CmdSetTarget(netIdentity);

                // call OnSelect + Hooks
                OnSelect();
                onSelect.Invoke();
            }
        }
    }

    protected virtual void OnSelect() {}

    //protected virtual void OnTriggerEnter(){}

}
