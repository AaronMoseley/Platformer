using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Governs the AI for this enemy in its entirety

    [Header("Physics/Collision Information")]
    public float normalSpeed;
    public float gravityForce;
    public int groundLayer;
    public string defaultMovingState;
    string movingState;
    [Space]

    [Header("Player Detection Information")]
    public int detectionLines;
    public float detectionDist;
    public float autoDetectRadius;
    public float coneSize;
    public float alertTime;
    public float playerChaseDist;
    public float stopChaseDist;
    float timeAlerted;
    string detectionState = "patrolling";
    int direction = 1;
    [Space]

    [Header("Patrolling Information")]
    public Transform[] patrolPos;
    public float patrolPosErr;
    public int currentPatrolPos = 0;
    GameObject lastKnownPos;
    [Space]

    [Header("Jumping Information")]
    public float jumpForce;
    public float jumpTime;
    public float jumpHorizontalForce;
    public float jumpYVelBaseline;
    public float jumpWaitTime;
    public int maxJumpCount;
    float currJumpTime;
    float currWaitTime;
    bool waitingForJump = false;
    List<float> jumpPos = new List<float>();
    [Space]

    GameObject player;

    Rigidbody2D rb;

    AIPath aiPath;
    AIDestinationSetter destinationSetter;

    GameObject gunManager;
    EnemyCollManager footCollider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        rb = gameObject.GetComponent<Rigidbody2D>();

        aiPath = gameObject.GetComponentInChildren<AIPath>();
        destinationSetter = gameObject.GetComponentInChildren<AIDestinationSetter>();

        gunManager = gameObject.GetComponentInChildren<EnemyGunManager>().gameObject;
        footCollider = gameObject.GetComponentInChildren<EnemyCollManager>();

        //Sets variables to their default values
        movingState = defaultMovingState;
        destinationSetter.target = patrolPos[currentPatrolPos];
        currWaitTime = jumpWaitTime;
    }

    void Update()
    {
        //Updates the detection state and resets the AI object's position to the enemy position
        bool detected = Detection();

        aiPath.gameObject.transform.localPosition = Vector2.zero;

        //Sets the direction based on the enemy's relation to its target
        if(destinationSetter.target.position.x > gameObject.transform.position.x)
        { 
            direction = 1;
        } else if(destinationSetter.target.position.x < gameObject.transform.position.x)
        {
            direction = -1;
        }

        if (detectionState.Equals("tracking"))
        {
            //If the enemy is tracking the player, increase the time alerted timer
            timeAlerted += Time.deltaTime;

            //If the enemy has been searching for too long or the player is too far away, stop tracking and return to patrolling
            if (timeAlerted > alertTime || Mathf.Abs(gameObject.transform.position.x - destinationSetter.target.position.x) < stopChaseDist)
            {
                StartPatrolling();
            }
        }

        //Draws a raycast from the enemy toward the player to establish line of sight
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, player.transform.position - gameObject.transform.position, Mathf.Infinity);
        string hitTag = hit.collider.tag;

        if ((((hitTag.Equals("Player") && detectionState.Equals("tracking"))) || detected) && (movingState.Equals("normal") || movingState.Equals("stationary")) && footCollider.GetColliding())
        {
            //If the enemy can see the player or is tracking and has LOS and is not jumping, start shooting the player
            StartShooting();
        }
        else if (detectionState.Equals("shooting") && footCollider.GetColliding() && (!hitTag.Equals("Player") || Vector2.Distance(gameObject.transform.position, player.transform.position) > playerChaseDist))
        {
            //If the enemy is shooting, on the ground, and either can't see the player or the player is too far away, start tracking and chasing the last known position of the player
            StartTracking();
        }

        //If the enemy is close enough to its target patrol point, advance to the next one
        if (Vector2.Distance(gameObject.transform.position, patrolPos[currentPatrolPos].position) <= patrolPosErr && detectionState.Equals("patrolling"))
        {
            //If the next patrol point would be outside the array, reset it
            if(currentPatrolPos >= patrolPos.Length - 1)
            {
                currentPatrolPos = 0;
            } else
            {
                currentPatrolPos++;
            }

            destinationSetter.target = patrolPos[currentPatrolPos];
        }
    }

    private void FixedUpdate()
    {
        //Check to see if this enemy can/needs to jump, set the velocity if this enemy is walking, and add the gravity force
        CheckForJump();

        if (movingState == "normal")
        {
            rb.velocity = aiPath.desiredVelocity;
        }

        rb.AddForce(new Vector2(0, -gravityForce));
    }

    void CheckForJump()
    {
        if (movingState.Equals("jumping") && currJumpTime < jumpTime && !detectionState.Equals("stationary"))
        {
            //If the enemy is in the middle of a jump and the jump time is not too large, continue adding force
            rb.AddForce(new Vector2(0, jumpForce));
            currJumpTime += Time.deltaTime;
            currWaitTime = 0;
        }
        else if (aiPath.desiredVelocity.y > jumpYVelBaseline && footCollider.GetColliding() && currWaitTime >= jumpWaitTime && !detectionState.Equals("stationary") && !detectionState.Equals("shooting"))
        {
            //If the desired velocity of the AI is upwards enough, the enemy is on the ground and is not currently shooting, begin jumping
            movingState = "jumping";
            currJumpTime = 0;

            //If the enemy is patrolling, add the position of the start of the jump to a list
            if (detectionState.Equals("patrolling"))
            {
                jumpPos.Add(gameObject.transform.position.x);
            }
        }

        //If the enemy is moving left and right while jumping, clear the list of jump positions
        bool temp = true;

        if (jumpPos.Count > 1)
        {
            for(int i = 1; i < jumpPos.Count; i++)
            {
                if(Mathf.Round(jumpPos[i]) != Mathf.Round(jumpPos[i - 1]))
                {
                    temp = false;
                    jumpPos.Clear();
                }
            }
        }

        //If the enemy has reached the peak of their jump, add the horizontal force in its desired direction, set it back to normal, and force it wait for a time to jump again
        if(currJumpTime >= jumpTime && !movingState.Equals("stationary"))
        {
            rb.AddForce(new Vector2(direction * jumpHorizontalForce, 0));
            movingState = "normal";
            waitingForJump = true;
            currJumpTime = 0;
        }

        //If the enemy has jumped and landed in the same x coordinate a certain amount of times and is patrolling, set it to stationary until it sees the player
        //FAILSAFE
        if (temp && jumpPos.Count >= maxJumpCount && detectionState.Equals("patrolling"))
        {
            destinationSetter.target = gameObject.transform;
            jumpPos.Clear();

            movingState = "stationary";
            detectionState = "stationary";
        }

        if (waitingForJump)
        {
            currWaitTime += Time.deltaTime;
        }
    }

    void StartShooting()
    {
        detectionState = "shooting";
        destinationSetter.target = gameObject.transform;
        currentPatrolPos = 0;
    }

    void StartTracking()
    {
        //Begin tracking the player by setting the last known position and making that the enemy's target
        detectionState = "tracking";
        movingState = "normal";
        lastKnownPos.transform.SetParent(null);
        lastKnownPos.transform.position = player.transform.position;
        destinationSetter.target = lastKnownPos.transform;
        timeAlerted = 0;
    }

    void StartPatrolling()
    {
        //Begins patrolling by setting the target to the first patrol position and resetting the last known position object
        detectionState = "patrolling";
        lastKnownPos.transform.SetParent(gameObject.transform);
        lastKnownPos.transform.localPosition = Vector2.zero;
        destinationSetter.target = patrolPos[0];
    }

    bool Detection ()
    {
        //Draws a cone of raycasts between one angle and another that go a certain distance, if they hit the player, this function returns true
        float angle = 0;

        //Decides the start angle of the cone
        if (direction == -1)
        {
            angle = coneSize * (-Mathf.PI / 4);
        } else if(direction == 1)
        {
            angle = coneSize * ((-5 * Mathf.PI) / 4);
        }

        for(int i = 0; i < detectionLines; i++)
        {
            //Draws each line at the specified angle
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            angle += (2 * (coneSize * (-Mathf.PI / 4))) / detectionLines;

            Vector2 dir = new Vector2(x, y);

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, dir, detectionDist);

            Debug.DrawLine(gameObject.transform.position, hit.point, Color.red);

            //If the raycast hits the player, return true
            if (hit)
            {
                if (hit.collider.gameObject.tag.Equals("Player"))
                {
                    Debug.DrawLine(gameObject.transform.position, hit.point, Color.red);
                    return true;
                }
            }
        }

        //Draws a circle around the enemy with raycasts and returns true if one hits the player
        angle = 0;

        for (int i = 0; i < detectionLines; i++)
        {
            //Draws each raycast in the circle
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            angle += (2 * Mathf.PI) / detectionLines;

            Vector2 dir = new Vector2(x, y);

            RaycastHit2D hit;

            //Makes the circle much larger if the enemy is already shooting
            if (detectionState.Equals("shooting"))
            {
                hit = Physics2D.Raycast(gameObject.transform.position, dir, detectionDist);
            } else
            {
                hit = Physics2D.Raycast(gameObject.transform.position, dir, autoDetectRadius);
            }

            if (hit)
            {
                if (hit.collider.gameObject.tag.Equals("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public string GetDetectionState()
    {
        return detectionState;
    }

    public int GetDirection()
    {
        return direction;
    }
}
