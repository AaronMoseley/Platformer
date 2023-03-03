using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollManager : MonoBehaviour
{
    //Compiles the information from all trigger colliders on the player and makes it usable in the movement script

    [Header("Collision Options")]
    public float coyoteJumpTime;
    [Space]

    //Feet, Left wall bottom, Right wall bottom, Left wall top, Right wall top
    public Collisions[] colliders;

    public int footCollPos = 0;

    //Feet, left, right
    bool[] bools;
    int[] collidingLayers;
    GameObject[] collidingObjects;

    void Start()
    {
        //Sets up all arrays and establishes the child collision scripts, as well as setting their coyote jump times
        colliders = new Collisions[gameObject.transform.childCount - 1];
        bools = new bool[3];
        collidingLayers = new int[3];
        collidingObjects = new GameObject[3];

        for(int i = 0; i < colliders.Length; i++)
        {
            colliders[i] = gameObject.transform.GetChild(i).gameObject.GetComponent<Collisions>();
        }

        colliders[footCollPos].GetComponent<Collisions>().SetCoyoteTime(coyoteJumpTime);
    }

    private void Update()
    {
        //Sets the arrays in this script to correspond to the values in the colliders
        UpdateColliders();
    }

    void UpdateColliders()
    {
        //Foot Collider
        bools[0] = colliders[0].colliding;
        collidingLayers[0] = colliders[0].collidingLayer;
        collidingObjects[0] = colliders[0].collidingObject;

        //Four side colliders
        for (int i = 1; i < 3; i++)
        {
            bools[i] = colliders[i].colliding && colliders[i + 2].colliding;

            if (bools[i])
            {
                collidingLayers[i] = colliders[i].collidingLayer;
                collidingObjects[i] = colliders[i].collidingObject;
            }
            else
            {
                collidingLayers[i] = -1;
                collidingObjects[i] = null;
            }
        }
    }

    public bool[] GetBools()
    {
        return bools;
    }

    public int[] GetLayers()
    {
        return collidingLayers;
    }

    public GameObject[] GetObjects()
    {
        return collidingObjects;
    }
}
