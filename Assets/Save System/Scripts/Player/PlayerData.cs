using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //Set of data relating to the player for saving

    public float xPos;
    public float yPos;
    public float zPos;

    public int currScene;

    public string modifiedDate;

    public bool grappler;

    public PlayerData(Vector3 pos, int newScene, string dateTime, bool hasGrappler)
    {
        //Establishes the position of the player, scene, and world time of this save file
        xPos = pos.x;
        yPos = pos.y;
        zPos = pos.z;

        currScene = newScene;

        modifiedDate = dateTime;

        grappler = hasGrappler;
    }
}
