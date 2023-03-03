using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UseItems : MonoBehaviour
{
    HotkeyManager hotkeyManager;
    InputManager input;
    GameObject player;

    public int healthPotionHealAmount;

    void Start()
    {
        hotkeyManager = gameObject.GetComponent<HotkeyManager>();
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        
    }

    public void UseItem(Item item, InvSlot slot)
    {
        for(int i = 0; i < hotkeyManager.numHotkeys; i++)
        {
            if(hotkeyManager.hotkeyTargets[i] == slot)
            {
                hotkeyManager.hotkeySlots[i].GetComponent<Image>().enabled = false;
                hotkeyManager.hotkeySlots[i].GetComponent<Image>().sprite = null;
            }
        }

        if(slot.selected)
        {
            slot.ChangeSelection();
        }
        
        Debug.Log(item.itemName + " Used");

        switch(item.itemName)
        {
            case "Health Potion":
                player.GetComponent<PlayerHealth>().AddHealth(healthPotionHealAmount);
                break;
        }
    }
}
