using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ItemDrag : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject slot;

    GameObject player;

    GameObject hoverSlot;

    public bool drop = false;
    public bool equip = false;
    public float zLayer;

    bool dontEnd = false;

    Inventory inventory;
    HotkeyManager hotkeyManager;

    void Start()
    {
        slot = GetComponentInParent<InvSlot>().gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<Inventory>();
        hotkeyManager = inventory.gameObject.GetComponent<HotkeyManager>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventory.canClose = false;
        gameObject.transform.SetParent(gameObject.GetComponentInParent<Canvas>().gameObject.transform);
        gameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dontEnd)
        {
            hoverSlot.GetComponent<InvSlot>().SwitchItem(slot.GetComponent<InvSlot>().item, slot);
        }

        inventory.canClose = true;
        gameObject.transform.SetParent(slot.transform);
        gameObject.transform.localPosition = Vector2.zero;

        if (equip && !drop)
        {
            if (slot.GetComponent<InvSlot>().item.equippable)
            {
                player.GetComponentInChildren<HotkeyManager>().EquipItem(slot.GetComponent<InvSlot>().item, true);
            }
            else if (slot.GetComponent<InvSlot>().item.usable)
            {
                inventory.GetComponent<UseItems>().UseItem(slot.GetComponent<InvSlot>().item, slot.GetComponent<InvSlot>());
                slot.GetComponent<InvSlot>().ResetItem();
            }
        }
        else if (drop)
        {
            Drop();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Inventory Manager" || collision.gameObject.tag == "Player")
        {
            drop = true;
            equip = false;
        }

        if (collision.gameObject.GetComponent<InvSlot>())
        {
            dontEnd = false;
            hoverSlot = null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Inventory Manager")
        {
            drop = false;
        }
        else if (collision.gameObject.tag == "Player")
        {
            drop = false;
            equip = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<InvSlot>())
        {
            dontEnd = true;
            hoverSlot = collision.gameObject;
        }
    }

    void Drop()
    {
        if (slot.GetComponent<InvSlot>() == hotkeyManager.currEquipped)
        {
            hotkeyManager.EquipItem(slot.GetComponent<InvSlot>().item, false);
            Destroy(player.GetComponentInChildren<GunManager>().transform.GetChild(0).gameObject);
        }

        if(slot.GetComponent<InvSlot>().selected)
        {
            slot.GetComponent<InvSlot>().ChangeSelection();
        }

        for(int i = 0; i < hotkeyManager.numHotkeys; i++)
        {
            if(slot.GetComponent<InvSlot>() == hotkeyManager.hotkeyTargets[i])
            {
                hotkeyManager.hotkeyTargets[i] = null;
                hotkeyManager.hotkeySlots[i].GetComponent<Image>().sprite = null;
                hotkeyManager.hotkeySlots[i].GetComponent<Image>().enabled = false;
            }
        }

        slot.GetComponent<InvSlot>().DropItem();
    }
}
