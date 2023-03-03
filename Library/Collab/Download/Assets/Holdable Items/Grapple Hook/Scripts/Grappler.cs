using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public GameObject hook;
    public AudioSource shootSound;
    public AudioSource retractSound;
    public AudioSource pullSound;

    public float grappleForce;

    public float retractTime;
    public float retractSpeed;
    float retractDist;
    Vector2 hookLastPos;
    public float retractErr;
    public float playerEndForce;

    public float pullSpeed;

    public float maxDist;

    public string state;

    bool colliding = false;

    GameObject parent;
    GameObject player;
    Rigidbody2D playerRB;
    InputManager input;
    Movement movement;
    Inventory inventory;

    public Vector2 hookStartPos;
    
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        movement = player.GetComponent<Movement>();
        inventory = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<Inventory>();

        hookStartPos = hook.transform.localPosition;

        state = "stationary";
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>().showing == "none")
        {
            if (Vector2.Distance(gameObject.transform.position, hook.transform.position) > maxDist)
            {
                hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                retractSound.Play();
                state = "retracting";
            }

            if (!inventory.invOpen)
            {
                if (input.ButtonDown("Retract") && state != "stationary")
                {
                    hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    playerRB.AddForce(new Vector2(Mathf.Sign(playerRB.velocity.x) * playerEndForce, Mathf.Sign(playerRB.velocity.y) * playerEndForce));
                    gameObject.GetComponent<Rope>().currTension = gameObject.GetComponent<Rope>().maxTension;
                    retractDist = Vector2.Distance(gameObject.transform.position, hook.transform.position);
                    hookLastPos = hook.transform.position;

                    if (Vector2.Distance(gameObject.transform.position, hook.transform.position) > retractErr)
                    {
                        retractSound.Play();
                    }

                    state = "retracting";
                }
                else if (hook.transform.parent)
                {
                    if (input.ButtonDown("Shoot") && hook.transform.parent.gameObject == gameObject && !colliding)
                    {
                        if (movement.state == "sprinting")
                        {
                            movement.state = "normal";
                            movement.currSpeed = movement.normalSpeed;
                        }
                        else if (movement.state == "grabbing")
                        {
                            movement.state = "normal";
                        }

                        if (movement.state == "normal")
                        {
                            hook.transform.SetParent(null);
                            hook.GetComponent<Rigidbody2D>().isKinematic = false;
                            hook.GetComponent<Rigidbody2D>().AddForce(transform.right * grappleForce);
                            inventory.grapplerNotShot = false;
                            shootSound.Play();
                            state = "shot";
                        }
                    }
                }
                else if (input.ButtonDown("Shoot") && (state == "landed" || state == "pulling"))
                {
                    state = "pulling";
                    pullSound.Play();
                }

                if (!input.Button("Shoot") && state == "pulling")
                {
                    playerRB.velocity = Vector2.zero;
                    pullSound.Stop();
                    state = "landed";
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(state == "retracting")
        {
            if(Vector2.Distance(gameObject.transform.position, hook.transform.position) > retractDist)
            {
                retractDist = Vector2.Distance(gameObject.transform.position, hook.transform.position);
            }

            hook.transform.position = Vector3.MoveTowards(hook.transform.position, gameObject.transform.position, retractSpeed);

            if (Mathf.Abs(hook.transform.position.x - gameObject.transform.position.x) <= retractErr && Mathf.Abs(hook.transform.position.y - gameObject.transform.position.y) <= retractErr)
            {
                state = "stationary";
                
                hook.GetComponent<Rigidbody2D>().isKinematic = true;
                hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                hook.transform.SetParent(gameObject.transform);
                hook.transform.localPosition = hookStartPos;
                hook.transform.localRotation = Quaternion.Euler(Vector3.zero);
                hook.GetComponent<Hook>().edgeCollider.enabled = false;
                hook.GetComponent<Hook>().radius = 0;
                inventory.grapplerNotShot = true;
            }
        }

        if (!inventory.invOpen)
        {
            if (state == "pulling")
            {
                player.GetComponent<Rigidbody2D>().velocity = (hook.transform.position - player.transform.position).normalized * pullSpeed;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            colliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            colliding = false;
        }
    }
}
