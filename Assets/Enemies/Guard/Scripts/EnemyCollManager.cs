using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollManager : MonoBehaviour
{
    //Shows when this enemy is colliding with the ground, enabling jumping

    [Header("Audio")]
    public AudioSource jumpSound;
    [Space]

    [Header("Ground Information")]
    public int groundLayer;
    [Space]

    bool colliding = false;

    public bool GetColliding()
    {
        return colliding;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Playes the landing audio when hitting ground, sets the colliding variable to true
        if(collision.gameObject.layer == groundLayer)
        {
            jumpSound.Play();
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Sets colliding to false when not colliding with ground
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = false;
        }
    }
}
