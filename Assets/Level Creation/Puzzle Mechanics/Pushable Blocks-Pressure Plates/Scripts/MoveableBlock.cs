using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : MonoBehaviour
{
    public int groundLayer = 8;

    public bool collWithGround = true;

    public float gravForce;

    public float failSafeError;

    //Right, left
    public MoveableBoxPushCollider[] colliders;

    InputManager input;
    Movement playerMovement;

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -gravForce));

        if ((colliders[0].collidingWith.CompareTag("Player") && input.AxisRaw("Horizontal") < 0) || (colliders[1].collidingWith.CompareTag("Player") && input.AxisRaw("Horizontal") > 0) && collWithGround && playerMovement.GetGrapplerState().Equals("stationary"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerMovement.pushSpeed * input.AxisRaw("Horizontal"), 0);
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            collWithGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            collWithGround = false;
        } else if(collision.gameObject.layer == 7)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 7;
        }
    }
}
