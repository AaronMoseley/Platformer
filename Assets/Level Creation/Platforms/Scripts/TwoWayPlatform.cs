using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayPlatform : MonoBehaviour
{
    //Moves a platform from its initial point to a target at a set speed when the player touches it

    [Header("End Position")]
    public Transform target;
    Vector2 startPos;
    Vector2 currTarget;
    [Space]

    [Header("Platform Move Information")]
    public float speed;
    public float stopDist;
    public float waitTime;
    [Space]

    public string state = "stationary";

    public float waitTimer = 0;

    public bool collWithPlayer = false;

    Vector2 dir;

    InGameMenuManager menu;

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
        startPos = gameObject.transform.position;
        currTarget = target.position;

        //The speed needed is extremely low, so this allows the inspector to look better
        speed /= 1000;

        //Sets the direction as the unit vector from the platform to its target
        dir = (currTarget - (Vector2)gameObject.transform.position).normalized;
    }

    private void Update()
    {
        if(state.Equals("waiting"))
        {
            waitTimer += Time.deltaTime;

            if(waitTimer >= waitTime && collWithPlayer)
            {
                state = "moving";
                waitTimer = 0;
            } else if(!collWithPlayer)
            {
                state = "stationary";
                waitTimer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        //If the platform is set to moving and the game isn't paused, move the platform
        if(state.Equals("moving") && menu.GetShowing().Equals("none"))
        {
            MovePlatform();
        }
    }

    void MovePlatform()
    {
        //Moves the platform in the direction based on the speed
        gameObject.transform.position += (Vector3)dir * speed;

        //If the platform is within the stop distance from the target, stop the platform
        if (Vector2.Distance(currTarget, gameObject.transform.position) <= stopDist)
        {
            state = "stationary";
            collWithPlayer = false;
            waitTimer = 0;

            if(currTarget == (Vector2)target.position)
            {
                currTarget = startPos;
            } else
            {
                currTarget = target.position;
            }

            dir = (currTarget - (Vector2)gameObject.transform.position).normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Begins to move if the player touches it
        if(collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            collWithPlayer = true;

            if (state.Equals("stationary"))
            {
                state = "waiting";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            collWithPlayer = false;
        }
    }
}
