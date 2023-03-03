using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    //Controls a locked door that changes to unlocked when a key touches it

    [Header("Art")]
    public Sprite unlockedDoor;
    public Sprite lockedDoor;
    [Space]

    [Header("Key Information")]
    public GameObject key;
    [Space]

    [Header("Locking Information")]
    public int forwardDir;
    [Space]

    GameObject player;

    bool open = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == key)
        {
            //If the key touches the door, disable all colliders on the door and drop the key

            Collider2D[] colliders = gameObject.GetComponents<Collider2D>();

            for(int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].isTrigger)
                {
                    colliders[i].enabled = false;
                }
            }

            open = true;

            gameObject.GetComponent<SpriteRenderer>().sprite = unlockedDoor;

            key.GetComponent<Rigidbody2D>().isKinematic = false;
            key.GetComponent<Key>().SetCarrying(false);
            key.GetComponent<Key>().enabled = false;
            key.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(player.GetComponent<KeyHolder>().DestroyKey(key));
            player.GetComponent<KeyHolder>().SetCurrKey(null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(open && collision.gameObject.CompareTag("Player"))
        {
            if ((collision.transform.position.x > gameObject.transform.position.x && forwardDir > 0) || (collision.transform.position.x < gameObject.transform.position.x && forwardDir < 0))
            {
                Collider2D[] colliders = gameObject.GetComponents<Collider2D>();

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (!colliders[i].isTrigger)
                    {
                        colliders[i].enabled = true;
                    }
                }

                open = false;

                gameObject.GetComponent<SpriteRenderer>().sprite = lockedDoor;
            }
        }
    }
}
