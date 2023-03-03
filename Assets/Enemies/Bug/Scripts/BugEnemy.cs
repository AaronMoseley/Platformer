using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEnemy : MonoBehaviour
{
    //Governs the basic AI for the bug enemy

    [Header("Art")]
    public Sprite[] frames;
    public float frameSwitchTime;
    int currFrame = 0;
    float frameTimer;
    [Space]

    [Header("Movement/Collisions Information")]
    public float speed;
    public int groundLayer = 8;
    int dir = 1;
    [Space]

    SpriteRenderer sprites;

    void Start()
    {
        sprites = gameObject.GetComponent<SpriteRenderer>();

        //Sets the intial direction to where the enemy is facing
        dir = (int)Mathf.Sign(gameObject.transform.localScale.x);
    }

    void Update()
    {
        AnimateEnemy();

        //If the enemy stops moving (usually because they hit a wall), switch its direction
        if (Mathf.Round(gameObject.GetComponent<Rigidbody2D>().velocity.x) == 0)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1, 1, 1);
            dir *= -1;
        }

        //Set the velocity of the enemy
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * dir, gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    void AnimateEnemy()
    {
        //If enough time has passed, the enemy sprite advances to the next in the array
        frameTimer += Time.deltaTime;

        if (frameTimer >= frameSwitchTime)
        {
            //If the next frame would be outside the array, reset it
            if(currFrame >= frames.Length - 1)
            {
                currFrame = -1;
            }

            currFrame++;

            sprites.sprite = frames[currFrame];

            frameTimer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //If the enemy walks to the end of a ledge, it turns around
        if(collision.gameObject.layer == groundLayer)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1, 1, 1);
            dir *= -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the enemy hits the player, it kills the player and stops moving
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            this.enabled = false;
        }
    }
}
