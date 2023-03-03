using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    //Simulates a realistic rope as a part of the grappling hook

    [Header("Rope Settings")]
    public Transform barrel;
    public int amtSegments;
    public float segLength;
    public float lineWidth;
    public float baseDiff;
    [Space]

    [Header("Physics Settings")]
    public float gravForce;
    public int brittleness;
    [Space]

    public float maxTension;
    public float minTension;
    public float tensionChangeRate;
    public float currTension;
    [Space]

    readonly List<RopeSegment> segments = new List<RopeSegment>();

    LineRenderer line;
    GameObject hook;
    Grappler grappler;
    GameObject player;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        grappler = gameObject.GetComponent<Grappler>();
        hook = grappler.hook;
        player = GameObject.FindGameObjectWithTag("Player");

        //Sets default values for the rope segments and line renderer
        for(int i = 0; i < amtSegments; i++)
        {
            segments.Add(new RopeSegment(Vector2.zero));
        }

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.positionCount = segments.Count;
        currTension = (maxTension + minTension) / 2;
    }

    void Update()
    {
        if (grappler.GetState().Equals("landed"))
        {
            //Changes the tension based on how far the player is from the edge of the rope
            currTension = Vector2.Distance(player.transform.position, hook.transform.position) / (hook.GetComponent<Hook>().radius + tensionChangeRate);

            currTension = Mathf.Clamp(currTension, minTension, maxTension);
        }

        if(!grappler.GetState().Equals("stationary"))
        {
            //Draws the rope if the grappler has been shot and decides the distance between line segments
            DrawRope();

            segLength = (Vector2.Distance(gameObject.transform.position, grappler.hook.transform.position) / amtSegments) + baseDiff;

        } else if(line.GetPosition(0) != Vector3.zero && line.GetPosition(line.positionCount - 1) != Vector3.zero)
        {
            //Removes the line from view when not shot 
            for(int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, Vector3.zero);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!grappler.GetState().Equals("stationary"))
        {
            //Simulate the physics on the line when the grappler has been shot
            Simulate();
        }
    }

    void DrawRope()
    {
        //Sets the correct position of the rope at the start and end
        line.SetPosition(0, barrel.transform.position);
        line.SetPosition(segments.Count - 1, hook.transform.position);

        for(int i = 1; i < segments.Count - 1; i++)
        {
            //Draws each segment of the line renderer based on the segments array
            line.SetPosition(i, segments[i].currPos);
        }
    }

    void Simulate()
    {
        //Simulates gravity force
        Vector2 gravVec = new Vector2(0, -gravForce);

        for(int i = 0; i < segments.Count; i++)
        {
            //Finds how fast each segment is moving and updates the position variables
            Vector2 vel = segments[i].currPos - segments[i].lastPos;
            segments[i].lastPos = segments[i].currPos;

            segments[i].currPos += vel + (gravVec * Time.deltaTime);
        }

        for(int i = 0; i < brittleness + 1; i++)
        {
            //Constrains each part of the rope to realistic rope physics
            Constrain();
        }
    }

    void Constrain()
    {
        //Forces the beginning and end of rope at the barrel and hook
        segments[segments.Count - 1].currPos = grappler.hook.transform.position;
        segments[0].currPos = barrel.transform.position;

        for(int i = 0; i < segments.Count - 1; i++)
        {
            //Finds whether the segments are too far or too close to each other
            float dist = Vector2.Distance(segments[i].currPos, segments[i + 1].currPos);
            float diff = Mathf.Abs(dist - segLength);

            //Determines the direction the segment needs to move in to be a part of the rope
            Vector2 newDir = Vector2.zero;

            if(diff != 0)
            {
                if (dist > segLength)
                {
                    newDir = (segments[i].currPos - segments[i + 1].currPos).normalized;
                } else if(dist < segLength)
                {
                    newDir = (segments[i + 1].currPos - segments[i].currPos).normalized;
                }
            }

            //Updates the position variables based on the new direction
            Vector2 changeAmt = newDir * diff;
            if (i != 0)
            {
                segments[i].currPos -= changeAmt * currTension;
                segments[i + 1].currPos += changeAmt * currTension;
            } else
            {
                segments[i + 1].currPos += changeAmt;
            }
        }
    }
}
