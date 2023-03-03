using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HotkeyManager : MonoBehaviour
{
    public int numHotkeys;
    public List<InvSlot> hotkeyTargets = new List<InvSlot>();
    public InvSlot currEquipped;
    public GameObject[] hotkeySlots;

    InputManager input;
    GameObject player;
    GameObject gunManager;
    GameObject hotbar;
    InGameMenuManager menu;

    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        gunManager = player.GetComponentInChildren<GunManager>().gameObject;
        hotbar = GameObject.FindGameObjectWithTag("Hotbar");
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();

        numHotkeys = input.hotkeys.Count;

        hotkeySlots = new GameObject[numHotkeys];

        for(int i = 0; i < numHotkeys; i++)
        {
            hotkeySlots[i] = hotbar.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < numHotkeys; i++)
        {
            hotkeyTargets.Add(null);
        }

        hotkeyTargets[0] = gunManager.GetComponent<InvSlot>();
    }

    void Update()
    {
        if (!gameObject.GetComponent<Inventory>().invOpen && menu.showing == "none")
        {
            for (int i = 0; i < numHotkeys; i++)
            {
                if (input.ButtonDown(input.hotkeys[i].name) && hotkeyTargets[i] != null)
                {
                    if (hotkeyTargets[i].equippable)
                    {
                        EquipItem(hotkeyTargets[i].item, !(hotkeyTargets[i] == currEquipped));

                        if (hotkeyTargets[i] == currEquipped)
                        {
                            currEquipped = null;
                        }
                        else
                        {
                            currEquipped = hotkeyTargets[i];
                        }
                    }
                    else if (hotkeyTargets[i].usable)
                    {
                        gameObject.GetComponent<UseItems>().UseItem(hotkeyTargets[i].item, hotkeyTargets[i]);
                        hotkeyTargets[i].gameObject.GetComponentInChildren<Text>().text = "";
                        hotkeyTargets[i].ResetItem();
                        hotkeyTargets[i] = null;
                    }
                } else if(input.ButtonDown(input.hotkeys[i].name))
                {
                    if (gunManager.transform.childCount > 0)
                    {
                        Destroy(gunManager.transform.GetChild(0).gameObject);
                        currEquipped = null;
                    }
                }
            }
        }
    }

    public void EquipItem (Item item, bool equip)
    {
        if (equip)
        {
            if(gunManager.transform.childCount > 0)
            {
                Destroy(gunManager.transform.GetChild(0).gameObject);

                if (currEquipped.itemName == "Grappler")
                {
                    Destroy(player.GetComponentInChildren<Grappler>().hook);
                }
            }

            GameObject equippedItem = Instantiate(item.equippedItem, gunManager.transform);
            equippedItem.transform.localPosition = new Vector2(gunManager.GetComponent<GunManager>().xOffset, gunManager.GetComponent<GunManager>().yOffset);
        } else
        {
            if(item.name == "Grappler")
            {
                Destroy(player.GetComponentInChildren<Grappler>().hook);
            }

            Destroy(gunManager.transform.GetChild(0).gameObject);
        }

        if(item.name == "Grappler")
        {
            player.GetComponent<Movement>().SetUpGrappler(equip);
        }
    }
}
