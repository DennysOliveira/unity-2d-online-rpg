using System;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Experience : NetworkBehaviour
{
    [Header("Components")]
    public Level level;


    [Header("Experience")]
    [SyncVar, SerializeField] long _current = 0;
    public long current
    {
        get { return _current; }
        set
        {
            if (value <= _current)
            {
                // decrease
                _current = Math.Max(value, 0);
            }
            else
            {
                // increase with level ups
                // set the new value (which might be more than expMax)
                _current = value;

                while(_current >= max)
                {
                    _current -= max;
                    ++level.current;

                    // call events
                    onLevelUp.Invoke();
                }

                // set to expMax if there is still too much exp remaining
                if (_current > max) _current = max;
            }
        }
    }

    // required experience grows by 10% each level
    [SerializeField] protected ExponentialLong _max = new ExponentialLong{multiplier=100, baseValue=1.1f};
    public long max { get { return _max.Get(level.current); } }

    [Header("Death")]
    public float deathLossPercent = 0.05f;

    [Header("Events")]
    public UnityEvent onLevelUp;

    // helper functions
    public float Percent() =>
        (current != 0 && max != 0) ? (float)current / (float)max : 0;

    // If in the future I choose to balance experience rewards, should do it here
    public static long BalanceExperienceReward(long reward, int attackerLevel, int victimLevel, int maxLevelDiff = 20)
    { return 1; }

    // |- Events ->
    [Server]
    public virtual void OnDeath()
    {
        // lose exp
        current -= Convert.ToInt64(max * deathLossPercent);
    }
}
