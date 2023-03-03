using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class SceneEndDoor : MonoBehaviour
{
    //Controls a door the player can enter to advance to the next scene or the last scene

    [Header("UI Settings")]
    public string interactMessage;
    [Space]

    [Header("Scene Advancement Settings")]
    public int direction;
    public Vector2 enterPos;
    public float waitTime;
    [Space]

    bool canAdvance = false;

    InputManager input;
    Text interactText;

    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        interactText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
    }

    void Update()
    {
        //If the player interacts with the door, it loads the new scene
        if(canAdvance && input.ButtonDown("Use"))
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + direction);
            StartCoroutine(LoadPlayer());
        }
    }

    IEnumerator LoadPlayer()
    {
        //This sets the player at the correct position in the new scene after a certain amount of time, without the wait, it doesn't work
        yield return new WaitForSeconds(waitTime);
        GameObject.FindGameObjectWithTag("Player").transform.position = enterPos;
        Destroy(gameObject);
    }

    //Allows the player to interact with the door
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            canAdvance = true;
            interactText.text = interactMessage;
            interactText.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            canAdvance = false;
            interactText.enabled = false;
            interactText.text = "";
        }
    }
}
