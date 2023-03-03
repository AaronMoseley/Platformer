using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Sign : MonoBehaviour
{
    //Lets the player view information on a sign

    [Header("Text Settings")]
    public string text;
    [Space]

    GameObject dialogueBackground;
    
    void Start()
    {
        dialogueBackground = GameObject.FindGameObjectWithTag("Dialogue Background");
    }

    //Displays the dialogue when the player enters the trigger, removes it when the player leaves
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            dialogueBackground.GetComponent<Image>().enabled = true;
            dialogueBackground.GetComponentInChildren<Text>().text = text;
            dialogueBackground.GetComponentInChildren<Text>().enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dialogueBackground.GetComponent<Image>().enabled = false;
            dialogueBackground.GetComponentInChildren<Text>().text = "";
            dialogueBackground.GetComponentInChildren<Text>().enabled = false;
        }
    }
}
