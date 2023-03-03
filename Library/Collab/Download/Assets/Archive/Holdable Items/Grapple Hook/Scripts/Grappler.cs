using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public GameObject hook;

    public float grappleForce;

    public float retractSpeed;
    public float retractErr;

    public float pullSpeed;

    public float maxDist;
    public float minDist;

    public string state;

    GameObject parent;
    GameObject player;
    InputManager input;
    Movement movement;

    Vector2 hookStartPos;

    bool doubleClickMouse1 = false;
    float mouse1DoubleClickTime;
    
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        movement = player.GetComponent<Movement>();

        hookStartPos = hook.transform.localPosition;

        state = "stationary";
    }

    void Update()
    {
        if(Vector2.Distance(gameObject.transform.position, hook.transform.position) > maxDist || (Vector2.Distance(gameObject.transform.position, hook.transform.position) < minDist && state == "landed"))
        {
            hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            state = "retracting";
        }

        if(doubleClickMouse1)
        {
            mouse1DoubleClickTime += Time.deltaTime;
        }

        if(doubleClickMouse1 && input.ButtonDown("Shoot") && mouse1DoubleClickTime <= input.doubleClickInterval)
        {
            mouse1DoubleClickTime = 0;
            doubleClickMouse1 = false;

            hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            state = "retracting";
        } else if (hook.transform.parent)
        {
            if (input.ButtonDown("Shoot") && hook.transform.parent.gameObject == gameObject)
            {
                if(movement.state == "crouching" || movement.state == "sliding")
                {
                    movement.state = "normal";
                    movement.Crouch();
                } else if(movement.state == "sprinting")
                {
                    movement.state = "normal";
                    movement.currSpeed = movement.normalSpeed;
                } else if(movement.state == "grabbing")
                {
                    movement.state = "normal";
                }

                hook.transform.SetParent(null);
                hook.GetComponent<Rigidbody2D>().isKinematic = false;
                hook.GetComponent<Rigidbody2D>().AddForce(transform.right * grappleForce);
                state = "shot";
            }
        } else if(input.Button("Shoot") && (state == "landed" || state == "pulling") && mouse1DoubleClickTime > input.doubleClickInterval)
        {
            state = "pulling";
        }
        
        if (doubleClickMouse1 && mouse1DoubleClickTime > input.doubleClickInterval)
        {
            mouse1DoubleClickTime = 0;
            doubleClickMouse1 = false;
        }

        if (!input.Button("Shoot") && state == "pulling")
        {
            state = "landed";
        }

        if (input.ButtonDown("Shoot") && !hook.transform.parent && (state != "retracting") && !doubleClickMouse1)
        {
            mouse1DoubleClickTime = 0;
            doubleClickMouse1 = true;
        }
    }

    private void FixedUpdate()
    {
        if(state == "retracting")
        {
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
            }
        }

        if(state == "pulling")
        {
            player.GetComponent<Rigidbody2D>().velocity = (hook.transform.position - player.transform.position).normalized * pullSpeed;
        }
    }
}
