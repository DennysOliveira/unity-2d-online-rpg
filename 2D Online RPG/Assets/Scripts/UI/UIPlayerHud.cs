using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPlayerHud : MonoBehaviour
{
    GameObject panel;
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider staminaSlider;
    public Slider experienceSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            healthSlider.value      = player.entity.curHealth;
            healthSlider.maxValue   = player.entity.maxHealth;

            manaSlider.value        = player.entity.curMana;
            manaSlider.maxValue     = player.entity.maxMana;

            staminaSlider.value     = player.entity.curStamina;
            staminaSlider.maxValue  = player.entity.maxStamina;

            experienceSlider.value = 0;
        }
    }
}
