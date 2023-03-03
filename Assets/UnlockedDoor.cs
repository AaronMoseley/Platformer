using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedDoor : MonoBehaviour
{
    //Controls a locked door that changes to unlocked when a key touches it

    [Header("Art")]
    public Sprite unlockedDoor;
    public Sprite lockedDoor;
    [Space]

    [Header("Locking Information")]
    public int forwardDir;
    [Space]

    bool open = false;

    private void Start()
    {
        //If the key touches the door, disable all colliders on the door and drop the key

        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
            {
                colliders[i].enabled = false;
            }
        }

        open = true;

        gameObject.GetComponent<SpriteRenderer>().sprite = unlockedDoor;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (open && collision.gameObject.CompareTag("Player"))
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

                gameObject.GetComponent<SpriteRenderer>().sprite = lockedDoor;
            }
        }
    }
}
