using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Key : MonoBehaviour
{
    //Controls a key and its movement while being held

    [Header("Carrying Information")]
    public Vector2 carryingOffset;
    public float carryResponsiveness;
    [Space]

    [Header("UI Settings")]
    public string interactMessage;
    [Space]

    bool carrying = false;
    bool canInteract = false;

    InputManager input;
    Text interactText;

    GameObject player;
    GameObject playerGFX;
    KeyHolder playerKeyHolder;

    Vector3 idealPos = Vector3.zero;

    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        interactText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerGFX = player.GetComponentInChildren<Blinking>().gameObject;
        playerKeyHolder = player.GetComponent<KeyHolder>();
    }

    void Update()
    {
        if(canInteract && input.ButtonDown("Use"))
        {
            //Picks up the key when interacted with
            carrying = true;
            canInteract = false;
            interactText.text = "";
            interactText.enabled = false;

            //Resets the old key and destroys it after a while, then sets this key as the current key
            if(playerKeyHolder.GetCurrKey() != null)
            {
                GameObject oldKey = player.GetComponent<KeyHolder>().GetCurrKey();

                oldKey.GetComponent<Key>().carrying = false;
                oldKey.GetComponent<Rigidbody2D>().isKinematic = false;
                oldKey.GetComponent<Key>().carrying = false;
                oldKey.GetComponent<Key>().enabled = false;
                oldKey.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(player.GetComponent<KeyHolder>().DestroyKey(oldKey));
            }

            playerKeyHolder.SetCurrKey(gameObject);
        }

        if(carrying && idealPos != Vector3.zero)
        {
            gameObject.transform.position = idealPos;
        }
    }

    private void FixedUpdate()
    {
        //Moves the key to the desired offset when being carried
        if (carrying)
        {
            idealPos = new Vector2(Mathf.Lerp(gameObject.transform.position.x, player.transform.position.x + (carryingOffset.x * Mathf.Sign(playerGFX.transform.localScale.x)), carryResponsiveness), Mathf.Lerp(gameObject.transform.position.y, player.transform.position.y + carryingOffset.y, carryResponsiveness));
        }
    }

    public void SetCarrying(bool state)
    {
        carrying = state;
    }

    //Logs whether the player can pick up the key and allows the player to interact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !carrying)
        {
            canInteract = true;
            interactText.text = interactMessage;
            interactText.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !carrying)
        {
            canInteract = false;
            interactText.text = "";
            interactText.enabled = false;
        }
    }
}
