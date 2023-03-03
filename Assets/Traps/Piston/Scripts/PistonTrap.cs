using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonTrap : MonoBehaviour
{
    //A trap that will crush the player at certain intervals

    [Header("Push Settings")]
    public Vector2 dir;
    public bool ignoreObjects;
    public float pushForce;
    public float retractForce;
    public float initDelay;
    public float normalDelay;
    public float resetError;
    public Vector2 target;
    [Space]

    [Header("Collision Settings")]
    public int groundLayer = 8;
    public float width;
    [Space]

    float timer = 0;
    Vector2 initLoc;

    string state = "stationary";

    LineRenderer arm;

    void Start()
    {
        initLoc = gameObject.transform.position;
        arm = gameObject.transform.parent.gameObject.GetComponent<LineRenderer>();
        arm.SetPosition(0, gameObject.transform.parent.position);

        if(dir.y == 0)
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        } else if(dir.x == 0)
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        if(ignoreObjects)
        {
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, dir, Mathf.Infinity, LayerMask.GetMask("Ground"));

            if(hit)
            {
                target = hit.point;
            }
        }
    }

    
    void Update()
    {
        //Increases the wait timer if the piston isn't retracting
        if (!state.Equals("retracting"))
        {
            timer += Time.deltaTime;
        }

        //Starts pushing the piston if it's waited long enough
        if((timer >= initDelay && initDelay >= 0) || (timer >= normalDelay && initDelay < 0))
        {
            state = "pushing";
            timer = 0;
            initDelay = -1;
        }

        if(ignoreObjects && Vector2.Distance(gameObject.transform.position, target) <= resetError + width)
        {
            state = "retracting";
        }

        gameObject.transform.parent.gameObject.GetComponent<LineRenderer>().SetPosition(1, (Vector2)gameObject.transform.position);
    }

    private void FixedUpdate()
    {
        //Updates the position of the pusher up or down depending on whether it's pushing or retracting
        if(state.Equals("pushing"))
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(dir * pushForce);
        } else if(state.Equals("retracting") && Vector2.Distance(gameObject.transform.position, initLoc) > resetError)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(-dir * retractForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Kills the player and resets the piston
        if(!collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject.transform.parent.gameObject && !ignoreObjects)
        {
            state = "retracting";
        } else if(collision.gameObject == gameObject.transform.parent.gameObject)
        {
            state = "stationary";
        } else if(collision.gameObject.CompareTag("Player") && !state.Equals("stationary"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }
    }
}
