using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public int numEdges;
    public float playerSize;

    public EdgeCollider2D edgeCollider;

    public float radius;

    public int groundLayer;

    GameObject grappler;
    GameObject player;

    void Start()
    {
        grappler = gameObject.transform.parent.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        edgeCollider = gameObject.GetComponentInChildren<EdgeCollider2D>();
    }

    void Update()
    {
        if(grappler.GetComponent<Grappler>().state == "pulling")
        {
            FormCollider();
        }

        if(grappler.GetComponent<Grappler>().state == "pulling" || grappler.GetComponent<Grappler>().state == "landed")
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y - gameObject.transform.position.y, player.transform.position.x - gameObject.transform.position.x)));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && grappler.GetComponent<Grappler>().state == "shot" && collision.gameObject != grappler && !collision.isTrigger && collision.gameObject.layer == groundLayer)
        {
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            FormCollider();
            edgeCollider.enabled = true;

            grappler.GetComponent<Grappler>().state = "landed";
        }
    }

    void FormCollider()
    {
        radius = Vector2.Distance(player.transform.position, gameObject.transform.position) + playerSize;

        Vector2[] points = new Vector2[numEdges + 1];

        for(int i = 0; i < numEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / numEdges;

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            points[i] = new Vector2(x, y);
        }
        points[numEdges] = points[0];

        edgeCollider.points = points;
    }
}
