using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    //A spike ball that will kill the player upon collision and moves between two points

    [Header("Moving Information")]
    public Transform target;
    public float speed;
    public float stopDist;
    [Space]

    Vector2 startPos;
    Vector2 currTarget;
    Vector2 dir;

    void Start()
    {
        //Finds the direction to its target and sets default values
        dir = (target.transform.position - gameObject.transform.position).normalized;
        currTarget = target.position;
        startPos = gameObject.transform.position;

        speed /= 1000;
    }

    private void FixedUpdate()
    {
        //Moves towards its target
        gameObject.transform.position += (Vector3)dir * speed;

        //If the spike ball is close enough to its target, it reverses and moves towards its starting position
        if (Vector2.Distance(currTarget, gameObject.transform.position) <= stopDist)
        {
            dir *= -1;

            if((Vector3)currTarget == target.position)
            {
                currTarget = startPos;
            } else
            {
                currTarget = target.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the spike ball hits the player, it kills the player
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }
    }
}
