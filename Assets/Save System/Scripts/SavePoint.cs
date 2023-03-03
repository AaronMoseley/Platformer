using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    //Allows the player to open up the save menu and save their progress

    [Header("UI Settings")]
    public string useMessage;
    [Space]

    bool canSave = false;

    Text pickupText;
    InputManager input;
    InGameMenuManager menu;
    
    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        pickupText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
    }

    void Update()
    {
        if(menu.GetShowing().Equals("saves") && input.ButtonDown("Escape"))
        {
            //Closes the save menu
            menu.ShowSaves(false);
            menu.SetShowing("none");
        }
        
        if(canSave && input.ButtonDown("Use"))
        {
            //Opens the save menu
            menu.ShowSaves(true);
        }
    }

    //Allows the player to interact with the save point when inside the trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            pickupText.text = useMessage;
            pickupText.enabled = true;
            canSave = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            pickupText.text = "";
            pickupText.enabled = false;
            canSave = false;
        }
    }
}
