using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    //Controls the dialogue system of any NPC with multiple sections of dialogue
    
    [Header("Text Parts")]
    public string[] dialogue;
    public string interactMessage;
    Text interactText;
    Text dialogueText;
    [Space]

    [Header("Dialogue Components")]
    public float timeBetweenCharacters;
    public Image dialogueBackground;
    int currDialogueIndex = 0;
    int currCharIndex = 0;
    float timer;
    [Space]

    string state = "none";
    //Prevents the player from accidentally skipping on the first frame the dialogue shows
    bool cantSkip = false;

    GameObject player;
    GameObject gunManage;

    InGameMenuManager menu;
    InputManager input;
    CinemachineVirtualCamera vCam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gunManage = player.GetComponentInChildren<GunManager>().gameObject;

        interactText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
        dialogueText = dialogueBackground.gameObject.GetComponentInChildren<Text>();

        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        vCam = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        //Resets the ability to skip
        cantSkip = false;

        if(state.Equals("can interact") && input.ButtonDown("Use") && menu.GetShowing().Equals("none"))
        {
            //Shows the dialogue if the player interacts
            ShowDialogue(true);
        }

        if(state.Equals("loading text"))
        {
            //If the dialogue is loading, add a character to the text after a certain amount of time
            if(dialogueText.text == dialogue[currDialogueIndex])
            {
                state = "showing text";
            } else
            {
                timer += Time.deltaTime;

                if(timer >= timeBetweenCharacters)
                {
                    dialogueText.text += dialogue[currDialogueIndex][currCharIndex];
                    currCharIndex++;
                    timer = 0;
                }
            }
        }

        //Advances the dialogue to the next string if a key is pressed and closes the dialogue if the dialogue is finished
        if (state.Equals("showing text") && Input.anyKeyDown)
        {
            currDialogueIndex++;

            if(currDialogueIndex >= dialogue.Length)
            {
                ShowDialogue(false);
            } else
            {
                state = "loading text";
                dialogueText.text = "";
                currCharIndex = 0;
            }
        }

        //Lets the player skip the current dialogue
        if (Input.anyKeyDown && state.Equals("loading text") && !cantSkip)
        {
            dialogueText.text = dialogue[currDialogueIndex];
        }
    }

    void ShowDialogue(bool showing)
    {
        //Sets the dialogue setup to whatever the desired state is and prevents the player from moving if the text is showing
        dialogueBackground.enabled = showing;
        dialogueText.enabled = showing;
        dialogueText.text = "";
        player.GetComponent<Movement>().enabled = !showing;
        player.GetComponent<Movement>().talking = showing;
        gunManage.SetActive(!showing);

        //Sets the correct state and camera follow
        if(showing)
        {
            state = "loading text";
            interactText.text = "";
            interactText.enabled = false;
            cantSkip = true;
            vCam.Follow = gameObject.transform;
        } else
        {
            state = "none";
            vCam.Follow = player.transform;
        }
    }

    //Sets the state as can interact or none depending on if the player is in the required trigger, lets the player interact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && currDialogueIndex < dialogue.Length)
        {
            interactText.text = interactMessage;
            interactText.enabled = true;
            state = "can interact";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            interactText.text = "";
            interactText.enabled = false;
            state = "none";
        }
    }
}
