using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public int amtSegments;
    public float segLength;
    public float lineWidth;

    public float baseDiff;

    public float gravForce;
    public int brittleness;

    public float maxTension;
    public float minTension;
    public float tensionChangeRate;
    public float currTension;

    public Transform barrel;

    LineRenderer line;

    List<RopeSegment> segments = new List<RopeSegment>();
    GameObject hook;
    Grappler grappler;
    GameObject player;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        grappler = gameObject.GetComponent<Grappler>();
        hook = grappler.hook;
        player = GameObject.FindGameObjectWithTag("Player");

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
        if (grappler.state == "landed")
        {
            currTension = Vector2.Distance(player.transform.position, hook.transform.position) / (hook.GetComponent<Hook>().radius + tensionChangeRate);

            currTension = Mathf.Clamp(currTension, minTension, maxTension);
        }

        if(grappler.state != "stationary")
        {
            DrawRope();

            if (grappler.state != "stationary")
            {
                segLength = (Vector2.Distance(gameObject.transform.position, grappler.hook.transform.position) / amtSegments) + baseDiff;
            }

        } else if(grappler.state != "shot" && line.GetPosition(0) != Vector3.zero && line.GetPosition(line.positionCount - 1) != Vector3.zero)
        {
            for(int i = 0; i < line.positionCount; i++)
            {
                line.SetPosition(i, Vector3.zero);
            }
        }
    }

    private void FixedUpdate()
    {
        if (grappler.state != "stationary")
        {
            Simulate();
        }
    }

    void DrawRope()
    {
        for(int i = 0; i < segments.Count; i++)
        {
            line.SetPosition(i, segments[i].currPos);
        }
    }

    void Simulate()
    {
        Vector2 gravVec = new Vector2(0, -gravForce);

        for(int i = 0; i < segments.Count; i++)
        {
            Vector2 vel = segments[i].currPos - segments[i].lastPos;
            segments[i].lastPos = segments[i].currPos;

            segments[i].currPos += vel + (gravVec * Time.deltaTime);
        }

        for(int i = 0; i < brittleness + 1; i++)
        {
            Constrain();
        }
    }

    void Constrain()
    {
        segments[segments.Count - 1].currPos = grappler.hook.transform.position;
        segments[0].currPos = barrel.transform.position;

        for(int i = 0; i < segments.Count - 1; i++)
        {
            float dist = Vector2.Distance(segments[i].currPos, segments[i + 1].currPos);
            float diff = Mathf.Abs(dist - segLength);

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
