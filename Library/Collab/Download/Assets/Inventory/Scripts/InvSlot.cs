using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InvSlot : MonoBehaviour
{
    public string itemName;
    public string itemDesc;
    public GameObject itemObject;
    public Item item;

    GameObject player;

    public Image itemIcon;

    public bool usable = false;
    public bool equippable = false;

    public Transform dropPoint;

    Image infoPanel;
    Text nameText;
    Text descText;

    public bool occupied = false;
    public bool selected = false;

    InputManager input;
    Inventory inventory;
    HotkeyManager hotkeyManager;
    EventTrigger eventTrigger;

    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();

        nameText = GameObject.FindGameObjectWithTag("Name Text").GetComponent<Text>();
        descText = GameObject.FindGameObjectWithTag("Desc Text").GetComponent<Text>();
        infoPanel = GameObject.FindGameObjectWithTag("Info Panel").GetComponent<Image>();

        dropPoint = GameObject.FindGameObjectWithTag("Drop Point").GetComponent<Transform>();

        player = GameObject.FindGameObjectWithTag("Player");

        inventory = gameObject.GetComponentInParent<Inventory>();
        hotkeyManager = gameObject.GetComponentInParent<HotkeyManager>();
        eventTrigger = gameObject.GetComponent<EventTrigger>();

        if(item != null)
        {
            AddItem(item);
        }
    }

    private void Update()
    {
        if (eventTrigger)
        {
            if (item == null && eventTrigger.enabled)
            {
                eventTrigger.enabled = false;
            }
            else if (!eventTrigger.enabled && itemName != "")
            {
                eventTrigger.enabled = true;
            }
        }

        if (selected)
        {
            for (int i = 1; i < input.hotkeys.Count; i++)
            {
                if (input.ButtonDown(input.hotkeys[i].buttonName))
                {
                    if (hotkeyManager.hotkeyTargets[i] == this)
                    {
                        hotkeyManager.hotkeySlots[i].GetComponent<Image>().sprite = null;
                        hotkeyManager.hotkeySlots[i].GetComponent<Image>().enabled = false;
                        hotkeyManager.hotkeyTargets[i] = null;
                    }
                    else
                    {

                        hotkeyManager.hotkeyTargets[i] = this;
                        hotkeyManager.hotkeySlots[i].GetComponent<Image>().sprite = item.itemIcon;
                        hotkeyManager.hotkeySlots[i].GetComponent<Image>().enabled = true;

                        for (int j = 0; j < hotkeyManager.numHotkeys; j++)
                        {
                            if(hotkeyManager.hotkeyTargets[j] == this && j != i)
                            {
                                hotkeyManager.hotkeyTargets[j] = null;
                                hotkeyManager.hotkeySlots[j].GetComponent<Image>().sprite = null;
                                hotkeyManager.hotkeySlots[j].GetComponent<Image>().enabled = false;
                            }
                        }
                    }
                }
            }
        }
    }

    public void AddItem(Item itemInfo)
    {
        occupied = true;

        itemObject = itemInfo.itemPrefab;

        itemName = itemInfo.itemName;
        itemDesc = itemInfo.itemDesc;

        item = itemInfo;

        usable = itemInfo.usable;
        equippable = itemInfo.equippable;

        if (itemInfo.itemIcon != null)
        {
            itemIcon.sprite = itemInfo.itemIcon;
        }

        if (gameObject.GetComponent<Button>() && gameObject.GetComponent<BoxCollider2D>() && gameObject.GetComponent<Image>())
        {
            gameObject.GetComponent<Button>().interactable = true;

            if (gameObject.GetComponent<Image>().enabled)
            {
                itemIcon.enabled = true;
            }
        }
    }

    public void ResetItem()
    {
        occupied = false;

        itemObject = null;

        itemName = "";
        itemDesc = "";

        itemIcon.sprite = null;
        itemIcon.enabled = false;

        item = null;

        usable = false;
        equippable = false;

        if (gameObject.GetComponent<Button>())
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void DropItem()
    {   
        Instantiate(itemObject, dropPoint.transform.position, Quaternion.Euler(Vector3.zero));
        ResetItem();
    }

    public void ChangeDesc ()
    {
        infoPanel.enabled = selected;

        if (infoPanel.enabled)
        {
            nameText.text = itemName;
            descText.text = itemDesc;
        } else
        {
            nameText.text = "";
            descText.text = "";
        }
    }

    public void ChangeSelection()
    {
        selected = !selected;

        ChangeDesc();

        for(int i = 0; i < inventory.slotAmt; i++)
        {
            if (inventory.invSlots[i].GetComponent<InvSlot>().gameObject != gameObject)
            {
                inventory.invSlots[i].GetComponent<InvSlot>().selected = false;
            }
        }
    }

    /*public void UseItem ()
    {
        if (selected && usable)
        {
             ChangeSelection();
             ResetItem();
        }
    }*/

    public void SwitchItem(Item newItem, GameObject slot)
    {   
        if(item != null && slot.GetComponent<InvSlot>().item != null)
        {
            Item tempItem = item;

            AddItem(newItem);
            slot.GetComponent<InvSlot>().AddItem(tempItem);
        } else if(item == null && slot.GetComponent<InvSlot>().item != null)
        {
            AddItem(newItem);
            slot.GetComponent<InvSlot>().ResetItem();
        }

        nameText.text = "";
        descText.text = "";
        infoPanel.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetComponentInChildren<ItemDrag>())
        {
            if (collision.gameObject.GetComponent<ItemDrag>() && collision.gameObject != GetComponentInChildren<ItemDrag>().gameObject)
            {
                if (!input.Button("Shoot"))
                {
                    SwitchItem(collision.gameObject.GetComponent<ItemDrag>().slot.GetComponent<InvSlot>().item, collision.gameObject.GetComponent<ItemDrag>().slot);
                }
            }
        }
    }
}