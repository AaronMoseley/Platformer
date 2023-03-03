using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    //Controls the behavior of the hook as well as the collider that generates the grappling hook effect

    [Header("Collider Settings")]
    public int numEdges;
    public float playerSize;
    public float radius;
    public int groundLayer;
    public float edgeCurveMagnitude;
    [HideInInspector]
    public EdgeCollider2D edgeCollider;
    [HideInInspector]
    public GameObject hookedOn;
    [Space]

    string grapplerState = "stationary";

    GameObject gfx;
    Grappler grappler;
    GameObject player;

    void Start()
    {
        grappler = gameObject.transform.parent.gameObject.GetComponent<Grappler>();
        player = GameObject.FindGameObjectWithTag("Player");
        edgeCollider = gameObject.GetComponentInChildren<EdgeCollider2D>();
        gfx = gameObject.GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    void Update()
    {
        grapplerState = grappler.GetState();

        //If the grappler is pulling, continuously update the collider
        if(grapplerState.Equals("pulling") || grapplerState.Equals("pulling object"))
        {
            FormCollider();
        }

        //If the grappler is pulling or has landed, rotate the hook towards it
        if(grapplerState.Equals("pulling") || grapplerState.Equals("landed") || grapplerState.Equals("pulling object"))
        {
            gfx.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y - gameObject.transform.position.y, player.transform.position.x - gameObject.transform.position.x)));
        }
    }

    void FormCollider()
    {
        //Finds the correct radius based on how far the hook is from the play and the player height
        radius = Vector2.Distance(player.transform.position, gameObject.transform.position) + playerSize;
        int tempEdges = numEdges + (int)(edgeCurveMagnitude * (radius / grappler.maxDist));

        Vector2[] points = new Vector2[tempEdges + 1];

        for (int i = 0; i < tempEdges; i++)
        {
            //Generates points around a circle for the edge collider to use
            float angle = 2 * Mathf.PI * i / tempEdges;

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            points[i] = new Vector2(x, y);
        }

        //Sets the end point of the collider as the beginning
        points[tempEdges] = points[0];

        //Updates the actual collider
        edgeCollider.points = points;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //When the grappler hits the ground after being shot, stop it from moving and form the collider
        if (collision.gameObject.CompareTag("Cant Grapple"))
        {
            grappler.Retract();
        }
        else if ((collision.gameObject.CompareTag("Moveable Block") || collision.gameObject.layer == groundLayer) && grapplerState.Equals("shot"))
        {
            hookedOn = collision.gameObject;

            if (collision.gameObject.CompareTag("Moveable Block"))
            {
                grappler.SetPulling(collision.gameObject);
            }

            if (!collision.gameObject.GetComponent<Grappler>())
            {
                gameObject.transform.SetParent(collision.gameObject.transform);
            }

            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            FormCollider();
            edgeCollider.enabled = true;

            grappler.SetState("landed");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject == hookedOn && !grapplerState.Equals("retracting"))
        {
            grappler.Retract();
        }
    }
}
