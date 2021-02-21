using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

[Serializable]
public class Entity
{
    [Header("Name")]
    public string name;

    [Header("Health")]
    [SyncVar] public int curHealth;
    [SyncVar] public int maxHealth;

    [Header("Mana")]
    [SyncVar] public int curMana;
    [SyncVar] public int maxMana;

    [Header("Stamina")]
    [SyncVar] public int curStamina;
    [SyncVar] public int maxStamina;

    [Header("Stats")]
    public int strength     = 1;
    public int intelligence = 1;
    public int agility      = 1;
    public int wisdom       = 1;
    public int damage       = 1;
    public int armour       = 1;
    public float speed      = 2.0f;
}
