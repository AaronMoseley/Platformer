using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    //Moves a platform from its initial point to a target at a set speed when the player touches it

    [Header("End Position")]
    public Transform target;
    [Space]

    [Header("Platform Move Information")]
    public float speed;
    public float stopDist;
    [Space]

    bool moving = false;
    Vector2 dir;

    InGameMenuManager menu;

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();

        //The speed needed is extremely low, so this allows the inspector to look better
        speed /= 1000;

        //Sets the direction as the unit vector from the platform to its target
        dir = (target.transform.position - gameObject.transform.position).normalized;
    }

    private void FixedUpdate()
    {
        //If the platform is set to moving and the game isn't paused, move the platform
        if(moving && menu.GetShowing().Equals("none"))
        {
            MovePlatform();
        }
    }

    void MovePlatform()
    {
        //Moves the platform in the direction based on the speed
        gameObject.transform.position += (Vector3)dir * speed;

        //If the platform is within the stop distance from the target, stop the platform
        if (Vector2.Distance(target.transform.position, gameObject.transform.position) <= stopDist)
        {
            moving = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Begins to move if the player touches it
        if(collision.gameObject.CompareTag("Player"))
        {
            moving = true;
        }
    }
}
