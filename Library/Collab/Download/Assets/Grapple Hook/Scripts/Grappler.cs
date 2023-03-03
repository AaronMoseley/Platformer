using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    //Manages the grappling hook and controls most of its functionality

    [Header("Audio")]
    public AudioSource shootSound;
    public AudioSource retractSound;
    public AudioSource pullSound;
    [Space]

    [Header("Grappler Settings")]
    public GameObject hook;
    public float grappleForce;
    public float retractTime;
    public float retractSpeed;
    public float retractErr;
    public float maxDist;
    public float pullSpeed;
    public float objectPullSpeed;
    public float initPullForce;

    //A force that applies to the player once they retract the hook
    public float playerEndForce;
    [Space]

    public string state;
    float pulledObjectGravityScale = -1;

    InputManager input;

    public List<GameObject> collidingWith = new List<GameObject>();
    GameObject pullingObject;
    GameObject player;
    Rigidbody2D playerRB;
    Movement movement;

    public Vector2 hookStartPos;
    
    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        movement = player.GetComponent<Movement>();

        //Sets default values
        hookStartPos = hook.transform.localPosition;
        state = "stationary";
    }

    void Update()
    {
        //Checks if the game is paused
        if (input.gameObject.GetComponent<InGameMenuManager>().GetShowing().Equals("none"))
        {
            //If the hook is too far away, it begins retracting
            if (Vector2.Distance(gameObject.transform.position, hook.transform.position) > maxDist)
            {
                StartCoroutine(WaitForRetract());
            }

            //If the player clicks the retract button and the grappler isn't already retracted, it begins retracting
            if (input.ButtonDown("Retract") && !state.Equals("stationary"))
            {
                Retract();
            }
            else if (hook.transform.parent)
            {
                if (input.ButtonDown("Shoot") && state.Equals("stationary") && collidingWith.Count == 0)
                {
                    //If the grappler is shot, reset the player's movement script to normal, then fire the grappler
                    string moveState = movement.GetState();

                    if (moveState.Equals("grabbing"))
                    {
                        moveState = "normal";
                    }

                    if (moveState.Equals("normal"))
                    {
                        gameObject.GetComponent<LineRenderer>().enabled = true;
                        hook.transform.SetParent(null);
                        hook.GetComponent<Rigidbody2D>().isKinematic = false;
                        hook.GetComponent<Rigidbody2D>().AddForce(transform.right * grappleForce);
                        player.GetComponent<CapsuleCollider2D>().enabled = true;
                        player.GetComponent<BoxCollider2D>().enabled = false;
                        shootSound.Play();
                        state = "shot";
                    }
                } else if (input.ButtonDown("Shoot") && pullingObject != null)
                {
                    //If the hook has landed on a pullable object and the player is holding the fire button, it sets the state to reflect that
                    state = "pulling object";
                    pulledObjectGravityScale = pullingObject.GetComponent<Rigidbody2D>().gravityScale;
                    pullingObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                    pullSound.Play();
                }
                else if (input.Button("Shoot") && hook.transform.parent.gameObject != gameObject && !state.Equals("pulling"))
                {
                    //If the hook is on a static object, the player pulls towards it when the player clicks the fire button
                    state = "pulling";
                    pullSound.Play();
                    //playerRB.AddForce((hook.transform.position - gameObject.transform.position).normalized * initPullForce);
                }
            }

            if (!input.Button("Shoot") && (state.Equals("pulling") || state.Equals("pulling object")))
            {
                //If the player has been pulling but isn't, stop the pull

                if (pullingObject)
                {
                    pullingObject.GetComponent<Rigidbody2D>().gravityScale = pulledObjectGravityScale;
                    pulledObjectGravityScale = -1;
                }

                playerRB.velocity = Vector2.zero;
                pullSound.Stop();
                state = "landed";
            }    
        }

        if(!state.Equals("pulling") && !state.Equals("pulling object") && pullSound.isPlaying)
        {
            pullSound.Stop();
        }
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case "retracting":
                //If the hook is retracting, move towards the barrel of the grappler
                hook.transform.position = Vector3.MoveTowards(hook.transform.position, gameObject.transform.position, retractSpeed);

                //If the hook is retracting and close enough to the barrel, reset the grappler
                if (Mathf.Abs(hook.transform.position.x - gameObject.transform.position.x) <= retractErr && Mathf.Abs(hook.transform.position.y - gameObject.transform.position.y) <= retractErr)
                {
                    state = "stationary";

                    gameObject.GetComponent<LineRenderer>().enabled = false;
                    hook.GetComponent<Rigidbody2D>().isKinematic = true;
                    hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    hook.transform.SetParent(gameObject.transform);
                    hook.transform.localPosition = hookStartPos;
                    hook.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    hook.GetComponent<Hook>().edgeCollider.enabled = false;
                    hook.GetComponent<Hook>().radius = 0;
                    hook.GetComponent<Hook>().hookedOn = null;

                    for(int i = 0; i < hook.transform.childCount; i++)
                    {
                        hook.transform.GetChild(i).localRotation = Quaternion.Euler(Vector3.zero);
                    }
                }
                break;
            case "pulling":
                //If the grappler is pulling the player, set the velocity of the player towards the hook

                if(hook.transform.position.y <= player.transform.position.y && player.GetComponentInChildren<CollManager>().GetBools()[0])
                {
                    playerRB.velocity = new Vector2(Mathf.Sign(hook.transform.position.x - player.transform.position.x) * pullSpeed, 0);
                } else
                {
                    player.GetComponent<Rigidbody2D>().velocity = (hook.transform.position - player.transform.position).normalized * pullSpeed;
                }

                Debug.Log(playerRB.velocity);
                break;
            case "pulling object":
                pullingObject.GetComponent<Rigidbody2D>().velocity = objectPullSpeed * (gameObject.transform.position - pullingObject.transform.position).normalized;
                break;
        }    
    }

    public void ResetPullingObject()
    {
        pullingObject = null;
        pulledObjectGravityScale = -1;
    }

    public string GetState()
    {
        return state;
    }

    public void SetState(string newState)
    {
        state = newState;
    }

    public void SetPulling(GameObject newObject)
    {
        pullingObject = newObject;
    }

    public void Retract()
    {
        //Applies a force to the player to make the movement more fluid, sets the rope to max tension, plays the audio, then sets the state to retracting
        hook.transform.SetParent(null);
        hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerRB.AddForce(new Vector2(Mathf.Sign(playerRB.velocity.x) * playerEndForce, Mathf.Sign(playerRB.velocity.y) * playerEndForce));
        gameObject.GetComponent<Rope>().currTension = gameObject.GetComponent<Rope>().maxTension;
        player.GetComponent<BoxCollider2D>().enabled = true;
        player.GetComponent<CapsuleCollider2D>().enabled = false;
        ResetPullingObject();

        if (Vector2.Distance(gameObject.transform.position, hook.transform.position) > retractErr)
        {
            retractSound.Play();
        }

        state = "retracting";
    }

    public GameObject GetPulling()
    {
        return pullingObject;
    }

    IEnumerator WaitForRetract()
    {
        yield return new WaitForEndOfFrame();

        hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        retractSound.Play();
        state = "retracting";
    }

    //Logs if the grappler is inside of something
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            collidingWith.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        for(int i = 0; i < collidingWith.Count; i++)
        {
            if(collidingWith[i] == collision.gameObject)
            {
                collidingWith.RemoveAt(i);
            }
        }
    }
}
