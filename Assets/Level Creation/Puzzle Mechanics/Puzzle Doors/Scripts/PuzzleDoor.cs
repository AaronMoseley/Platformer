using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    //This door is activated by some object (pressure plate, lever, etc.) and then moves a specified way, to be used in puzzles
    
    [Header("Where the Door Moves")]
    public Vector2 openOffset;
    Vector2 startPos;
    [Space]

    [Header("How the Door Moves")]
    public float openSpeed;
    public float moveError;
    [Space]

    string state = "stationary";

    Vector2 currTarget = Vector2.zero;
    
    void Start()
    {
        //Logs the beginning position of the door for when it closes
        startPos = gameObject.transform.position;

        //Allows the inspector to not show extremely low decimals
        openSpeed /= 100;
    }

    void FixedUpdate()
    {
        //If the door isn't stationary, move it towards its target
        if(!state.Equals("stationary"))
        {
            gameObject.transform.position = new Vector2(Mathf.MoveTowards(gameObject.transform.position.x, currTarget.x, openSpeed), Mathf.MoveTowards(gameObject.transform.position.y, currTarget.y, openSpeed));

            //If the door is close enough to its target, set it at the target and stop it moving
            if (Vector2.Distance(gameObject.transform.position, currTarget) <= moveError)
            {
                state = "stationary";
                gameObject.transform.position = currTarget;
            }
        }
    }

    public void Open(bool temp)
    {
        //Sets the current target as either the open or close position and gets the door moving
        state = "moving";
        
        if(temp)
        {
            currTarget = startPos + openOffset;
        } else if(!temp)
        {
            currTarget = startPos;
        }
    }
}
