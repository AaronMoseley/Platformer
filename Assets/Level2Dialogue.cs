using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Level2Dialogue : MonoBehaviour
{
    //Controls the dialogue system of any NPC with multiple sections of dialogue

    [Header("Text Parts")]
    public string[] dialogue;
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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gunManage = player.GetComponentInChildren<GunManager>().gameObject;

        dialogueText = dialogueBackground.gameObject.GetComponentInChildren<Text>();
    }

    void Update()
    {
        //Resets the ability to skip
        cantSkip = false;

        if (state.Equals("loading text"))
        {
            //If the dialogue is loading, add a character to the text after a certain amount of time
            if (dialogueText.text == dialogue[currDialogueIndex])
            {
                state = "showing text";
            }
            else
            {
                timer += Time.deltaTime;

                if (timer >= timeBetweenCharacters)
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

            if (currDialogueIndex >= dialogue.Length)
            {
                ShowDialogue(false);
            }
            else
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
        if (showing)
        {
            state = "loading text";
            cantSkip = true;
        }
        else
        {
            state = "none";
        }
    }

    //Sets the state as can interact or none depending on if the player is in the required trigger, lets the player interact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ShowDialogue(true);
        }
    }
}
