using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment 
{
    //Scriptable object to be used in the rope

    public Vector2 currPos;
    public Vector2 lastPos;

    public RopeSegment(Vector2 pos)
    {
        //Gives the current and previous position to establish velocity
        currPos = pos;
        lastPos = pos;
    }
}
