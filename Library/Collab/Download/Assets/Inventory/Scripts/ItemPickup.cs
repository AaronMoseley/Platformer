using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item itemInfo;
    Text pickupText;

    InputManager input;
    Inventory inventory;

    public bool canPickUp = false;

    void Start()
    {
        pickupText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        inventory = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<Inventory>();
    }

    void Update()
    {
        if (input.Button("Use") && canPickUp && pickupText.text == itemInfo.pickupMessage + itemInfo.itemName && !inventory.invOpen)
        {
            canPickUp = false;

            for (int i = 0; i < inventory.slotAmt; i++)
            {
                if (!inventory.invSlots[i].GetComponent<InvSlot>().occupied)
                {
                    inventory.invSlots[i].GetComponent<InvSlot>().AddItem(itemInfo);
                    pickupText.enabled = false;
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && canPickUp)
        {
            pickupText.enabled = false;
            canPickUp = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !pickupText.enabled)
        {
            pickupText.enabled = true;
            pickupText.text = itemInfo.pickupMessage + itemInfo.itemName;
            canPickUp = true;
        }
    }
}