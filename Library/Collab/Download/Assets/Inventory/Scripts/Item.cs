using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory", order = 1)]
public class Item : ScriptableObject
{
    public GameObject itemPrefab;

    public string itemName;
    public string itemDesc;
    public string pickupMessage = "Press USE to pick up ";

    public Sprite itemIcon;

    public bool usable;

    public bool equippable;
    public GameObject equippedItem;

    public string itemID;
}