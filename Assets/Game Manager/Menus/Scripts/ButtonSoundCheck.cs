using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ButtonSoundCheck : MonoBehaviour
{
    //Checks to see if the button is interactable and disables the sound effect if it isn't

    Button button;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        //Disables or enables the event trigger depending on the button's interactibility
        if(!button.interactable && gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = false;
        } else if(gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = true;
        }
    }
}
