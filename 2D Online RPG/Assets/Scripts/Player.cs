using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    public Entity entity;
    
    
    [Header("Player UI")]
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider staminaSlider;
    public Slider expSlider;
    public Canvas gameCanvas;

    void Start()
    {
        gameCanvas = GameObject.FindWithTag("GameCanvas").GetComponent<Canvas>();
        
        if(!isLocalPlayer)
            Destroy(gameCanvas);
        
        if(isLocalPlayer)
        {
            // Grab slider references // GameObject.FindWithTag("HealthSlider").GetComponent<Slider>();
            healthSlider = GameObject.FindWithTag("HealthSlider").GetComponent<Slider>();
            manaSlider = GameObject.FindWithTag("ManaSlider").GetComponent<Slider>();
            staminaSlider = GameObject.FindWithTag("StaminaSlider").GetComponent<Slider>();
            expSlider = GameObject.FindWithTag("ExperienceSlider").GetComponent<Slider>();

            // Sets up entity values according to the Database
            CmdSetEntityValues();

            // Set slider values according to Entity values
            healthSlider.value      = entity.curHealth;
            healthSlider.maxValue   = entity.maxHealth;

            manaSlider.value        = entity.curMana;
            manaSlider.maxValue     = entity.maxMana;

            staminaSlider.value     = entity.curStamina;
            staminaSlider.maxValue  = entity.maxStamina;

            expSlider.value = 0;
        }
        

    }

    
    private void Update()
    {
        healthSlider.value  = entity.curHealth;
        manaSlider.value    = entity.curMana;
        staminaSlider.value = entity.curStamina;

        if(Input.GetKeyDown(KeyCode.Space))
        {
           CmdTakeDamage(10);
        }
    }

    [Command]
    void CmdSetEntityValues()
    {
        // TO-DO: Get values from the database
        // int dbHealth  = GetValue("health", entity.name);
        // int dbMana    = GetValue("mana", entity.name);
        // int dbStamina = GetValue("stamina", entity.name);

        // Set HP/MP/SP values accordingly
        entity.curHealth  = entity.maxHealth;
        entity.curMana    = entity.maxMana;
        entity.curStamina = entity.maxStamina;
    }

    // Commands to get values from the database through the Server
    // In the future, should get current value from the database when the player logs in.
    [Command]
    void CmdGetHealth()
    {
        entity.curHealth = entity.maxHealth;
    }

    [Command]
    void CmdGetMana()
    {
        entity.curHealth = entity.maxHealth;
    }

    [Command]
    void CmdGetStamina()
    {
        entity.curStamina = entity.maxStamina;
    }

    [Command]
    void CmdGetExp()
    {
        Debug.Log("Getting EXP Values from the Database...");
    }

    [Command]
    void CmdTakeDamage(int dmg)
    {
        entity.curHealth -= dmg;
    }
}
