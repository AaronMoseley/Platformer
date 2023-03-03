using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment 
{
    public Vector2 currPos;
    public Vector2 lastPos;

    public RopeSegment(Vector2 pos)
    {
        currPos = pos;
        lastPos = pos;
    }
}
