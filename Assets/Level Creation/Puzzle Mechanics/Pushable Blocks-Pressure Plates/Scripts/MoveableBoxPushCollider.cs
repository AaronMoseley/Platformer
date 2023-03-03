using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBoxPushCollider : MonoBehaviour
{
    public GameObject collidingWith;

    private void Start()
    {
        collidingWith = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            collidingWith = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collidingWith == collision.gameObject)
        {
            collidingWith = gameObject;
        }
    }
}
