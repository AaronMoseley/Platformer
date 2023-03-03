using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject enemyGFX;
    GameObject gunManager;

    public Sprite normal;
    public Sprite blink;

    public float blinkTime;
    public float timeBetweenBlinks;
    bool blinking;
    float blinkTimer;
    
    public float gravityForce;
    public int groundLayer;
    public float normalSpeed;

    public int detectionLines;
    public float detectionDist;
    public float detectionRadius;
    public float coneSize;
    public float alertTime;
    float timeAlerted;

    public string defaultMovingState;
    public string detectionState = "patrolling";
    public string movingState;

    public Transform[] patrolPos;
    public float patrolPosErr;
    public int currentPatrolPos = 0;
    public GameObject lastKnownPos;
    public float playerChaseDist;
    public float stopChaseDist;

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

    public int direction = 1;

    GameObject player;
    EnemyCollManager footCollider;
    AIPath aiPath;
    AIDestinationSetter destinationSetter;
    Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        aiPath = gameObject.GetComponentInChildren<AIPath>();
        destinationSetter = gameObject.GetComponentInChildren<AIDestinationSetter>();
        footCollider = gameObject.GetComponentInChildren<EnemyCollManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        gunManager = gameObject.GetComponentInChildren<EnemyGunManager>().gameObject;

        movingState = defaultMovingState;
        destinationSetter.target = patrolPos[currentPatrolPos];
        currWaitTime = jumpWaitTime;
    }

    void Update()
    {
        if(Mathf.Abs(gunManager.GetComponent<EnemyGunManager>().correctAngle) > 90)
        {
            enemyGFX.transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            enemyGFX.transform.localScale = new Vector3(1, 1, 1);
        }

        blinkTimer += Time.deltaTime;

        if (!blinking && blinkTimer >= timeBetweenBlinks)
        {
            blinking = true;
            blinkTimer = 0;

            enemyGFX.GetComponent<SpriteRenderer>().sprite = blink;
        }
        else if (blinking && blinkTimer >= blinkTime)
        {
            blinking = false;
            blinkTimer = 0;

            enemyGFX.GetComponent<SpriteRenderer>().sprite = normal;
        }

        bool detected = Detection();

        aiPath.gameObject.transform.localPosition = Vector2.zero;

        if(destinationSetter.target.position.x > gameObject.transform.position.x)
        { 
            direction = 1;
        } else if(destinationSetter.target.position.x < gameObject.transform.position.x)
        {
            direction = -1;
        }

        if (detectionState == "tracking")
        {
            timeAlerted += Time.deltaTime;

            if (timeAlerted > alertTime || Mathf.Abs(gameObject.transform.position.x - destinationSetter.target.position.x) < stopChaseDist)
            {
                detectionState = "patrolling";
                lastKnownPos.transform.SetParent(gameObject.transform);
                destinationSetter.target = patrolPos[0];
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, player.transform.position - gameObject.transform.position, Mathf.Infinity);

        if ((((hit.collider.tag == "Player" && detectionState == "tracking")) || detected) && (movingState == "normal" || movingState == "stationary") && footCollider.colliding)
        {
            detectionState = "shooting";
            destinationSetter.target = gameObject.transform;
            currentPatrolPos = 0;
        }
        else if (detectionState == "shooting" && footCollider.colliding && (hit.collider.gameObject.tag != "Player" || Vector2.Distance(gameObject.transform.position, player.transform.position) > playerChaseDist))
        {
            detectionState = "tracking";
            movingState = "normal";
            lastKnownPos.transform.SetParent(null);
            lastKnownPos.transform.position = player.transform.position;
            destinationSetter.target = lastKnownPos.transform;
            timeAlerted = 0;
        }

        if (Vector2.Distance(gameObject.transform.position, patrolPos[currentPatrolPos].position) <= patrolPosErr && detectionState == "patrolling")
        {
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
        Jump();

        if (movingState == "normal")
        {
            rb.velocity = aiPath.desiredVelocity;
        }

        rb.AddForce(new Vector2(0, -gravityForce));
    }

    void Jump()
    {
        if (movingState == "jumping" && currJumpTime < jumpTime && detectionState != "stationary")
        {
            rb.AddForce(new Vector2(0, jumpForce));
            currJumpTime += Time.deltaTime;
            currWaitTime = 0;
        }
        else if (aiPath.desiredVelocity.y > jumpYVelBaseline && footCollider.colliding && currWaitTime >= jumpWaitTime && detectionState != "stationary")
        {
            movingState = "jumping";
            currJumpTime = 0;
            jumpPos.Add(gameObject.transform.position.x);
        }

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

        if(waitingForJump)
        {
            currWaitTime += Time.deltaTime;
        }

        if(currJumpTime >= jumpTime && movingState != "stationary")
        {
            rb.AddForce(new Vector2(direction * jumpHorizontalForce, 0));
            movingState = "normal";
            waitingForJump = true;
            currJumpTime = 0;
        }

        if (temp && jumpPos.Count >= maxJumpCount && detectionState == "patrolling")
        {
            destinationSetter.target = gameObject.transform;
            jumpPos.Clear();

            movingState = "stationary";
            detectionState = "stationary";
        }
    }

    bool Detection ()
    {
        float angle = 0;

        if (direction == -1)
        {
            angle = coneSize * (-Mathf.PI / 4);
        } else if(direction == 1)
        {
            angle = coneSize * ((-5 * Mathf.PI) / 4);
        }

        for(int i = 0; i < detectionLines; i++)
        {
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            angle += (2 * (coneSize * (-Mathf.PI / 4))) / detectionLines;

            Vector2 dir = new Vector2(x, y);

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, dir, detectionDist);

            Debug.DrawLine(gameObject.transform.position, hit.point, Color.red);

            if (hit)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.DrawLine(gameObject.transform.position, hit.point, Color.red);
                    return true;
                }
            }
        }

        angle = 0;

        for (int i = 0; i < detectionLines; i++)
        {
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            angle += (2 * Mathf.PI) / detectionLines;

            Vector2 dir = new Vector2(x, y);

            RaycastHit2D hit;
            if (detectionState != "shooting")
            {
                hit = Physics2D.Raycast(gameObject.transform.position, dir, detectionRadius);
            } else
            {
                hit = Physics2D.Raycast(gameObject.transform.position, dir, detectionDist);
            }

            if (hit)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    return true;
                }
            }
        }

        return false;
    }
}
