using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool invOpen = false;
    public bool canClose = true;
    public int slotAmt;

    public GameObject[] invSlots;

    public bool grapplerNotShot = true;

    Image invBackground;

    GameObject gameManager;
    InputManager input;

    GameObject infoPanel;
    GameObject nameText;
    GameObject descText;
    GameObject hotbar;

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        input = gameManager.GetComponent<InputManager>();
        invBackground = gameObject.GetComponent<Image>();
        invSlots = new GameObject[slotAmt];

        infoPanel = GameObject.FindGameObjectWithTag("Info Panel");
        nameText = GameObject.FindGameObjectWithTag("Name Text");
        descText = GameObject.FindGameObjectWithTag("Desc Text");

        hotbar = GameObject.FindGameObjectWithTag("Hotbar");

        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            if(gameObject.transform.GetChild(i).gameObject.GetComponent<InvSlot>())
            {
                invSlots[i] = gameObject.transform.GetChild(i).gameObject;
            }
        }
    }
    
    void LateUpdate()
    {
        if((input.ButtonDown("Inventory") || (input.ButtonDown("Cancel") && invOpen)) && canClose && grapplerNotShot && gameManager.GetComponent<InGameMenuManager>().showing == "none")
        {
            invOpen = !invOpen;
            
            invBackground.enabled = invOpen;
            infoPanel.GetComponent<Image>().enabled = false;
            nameText.GetComponent<Text>().text = "";
            descText.GetComponent<Text>().text = "";

            hotbar.GetComponent<Image>().enabled = invOpen;

            for(int i = 0; i < hotbar.transform.childCount; i++)
            {
                hotbar.transform.GetChild(i).gameObject.SetActive(invOpen);
            }

            for(int i = 0; i < slotAmt; i++)
            {
                invSlots[i].GetComponent<InvSlot>().selected = false;
                invSlots[i].SetActive(invOpen);
            }
        }
    }
}
