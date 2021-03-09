using UnityEngine;
using UnityEngine.Events;
using Mirror;

public abstract class Energy : NetworkBehaviour
{
    // current value
    // set & get: keep between min and max

    [SyncVar] int _current = 0;

    public int current
    {
        get { return Mathf.Min(_current, max); }
        set 
        {
            bool emptyBefore = _current == 0;
            _current = Mathf.Clamp(value, 0, max);
            if(_current == 0 && !emptyBefore) 
                onEmpty.Invoke();
        }
    }

    // maximum value (may depend on items, stats, buffs, etc.)
    public abstract int max { get; }

    // recovery rate (may depend on stats, equips, etc.)
    public abstract int recoveryRate { get; }

    // don't recover Energies while dead, so Energies need to check on Health.
    public Health health;

    // should this entity spawn with full Energy? (useful for monsters spawning etc)
    public bool spawnFull = true;

    [Header("Events")]
    public UnityEvent onEmpty;

    public override void OnStartServer()
    {
        // set full energy on spawn if needed
        if (spawnFull) current = max;

        // recovery every second
        InvokeRepeating(nameof(Recover), 1f, 1f);
    }

    // get current as percentage
    public float Percent() =>
        (current != 0 && max != 0) ? (float)current / (float)max : 0;

    // Recover method to be Invoked once a second
    [Server]
    public void Recover()
    {
        if (enabled && health.current > 0)
            current += recoveryRate;
    }


}
