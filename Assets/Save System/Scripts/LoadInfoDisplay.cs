using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class LoadInfoDisplay : MonoBehaviour
{
    //Displays information about a save file when the player is looking at saves

    [Header("UI Elements")]
    public Text displayText;
    [Space]

    [Header("Save Settings")]
    public int saveNum;
    //This script is attached to load and save buttons which require different actions
    public bool loadButton;
    [Space]

    AreaList areas;

    void Start()
    {
        areas = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<AreaList>();

        UpdateText();
    }

    void LateUpdate()
    {
        if (gameObject.GetComponent<Button>().interactable && loadButton && PlayerSaveLoad.LoadPlayer(saveNum) == null)
        {
            //If this is a load buttons and there's no save file, make the button not interactable
            gameObject.GetComponent<Button>().interactable = false;
        }

        //Makes the event trigger (sound effects) react to whether the button is interactable
        if(!gameObject.GetComponent<Button>().interactable && gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = false;
        } else 
        {
            gameObject.GetComponent<EventTrigger>().enabled = true;
        }
    }

    public void UpdateText()
    {
        //Displays information about the save file in the display text
        if (PlayerSaveLoad.LoadPlayer(saveNum) != null)
        {
            PlayerData data = PlayerSaveLoad.LoadPlayer(saveNum);
            int areaNum = data.currScene;
            string lastChanged = data.modifiedDate;

            displayText.text = "Area: " + areas.areas[areaNum] + "\nLast Modified: " + lastChanged;
        }
    }
}
