using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    //This script is placed on the trigger colliders on the player and logs when the player is touching objects at certain points

    public int collidingLayer;
    public bool colliding = false;
    public GameObject collidingObject;

    float coyoteJumpTime = 0;

    IEnumerator EndColl()
    {
        //After a specified time, reset the information (allows for the player to jump slightly after leaving the ground)
        yield return new WaitForSecondsRealtime(coyoteJumpTime);
        colliding = false;
        collidingLayer = -1;
        collidingObject = null;
    }

    public void SetCoyoteTime(float time)
    {
        coyoteJumpTime = time;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //If an object collides with this, log its information
        if(!collision.gameObject.CompareTag("Player") && !collision.isTrigger && !collision.gameObject.GetComponent<Hook>())
        {
            colliding = true;
            collidingLayer = collision.gameObject.layer;
            collidingObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //If the collision leaves, begin to end it
        if (!collision.gameObject.CompareTag("Player") && !collision.isTrigger && !collision.gameObject.GetComponent<Hook>())
        {
            StartCoroutine(EndColl());
        }
    }
}
