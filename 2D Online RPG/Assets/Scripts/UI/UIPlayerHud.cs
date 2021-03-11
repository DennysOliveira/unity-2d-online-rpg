using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPlayerHud : MonoBehaviour
{
    public GameObject panel;
    public Slider healthSlider;
    public Text healthText;
    public Slider manaSlider;
    public Text manaText;
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
            panel.SetActive(true);
            healthSlider.value      = player.health.current;
            healthSlider.maxValue   = player.health.max;
            healthText.text = player.health.current + " / " + player.health.max;

            manaSlider.value        = player.mana.current;
            manaSlider.maxValue     = player.mana.max;
            manaText.text = player.mana.current + " / " + player.mana.max;

            staminaSlider.value     = 0;
            staminaSlider.maxValue  = 100;

            experienceSlider.value = 0;
        }
        else panel.SetActive(false);
    }
}
