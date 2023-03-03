using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatforms : MonoBehaviour
{
    //Allows for the constant rotation of multiple objects at a certain radius and speed

    [Header("Included Platforms/Objects")]
    public GameObject[] platforms;
    [Space]

    [Header("Rotation Information")]
    public float rotateSpeed;
    public float radius;
    [Space]

    float startingAngle = 0;

    InGameMenuManager menu;

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();

        //Allows the inspector to not show extremely small decimals
        rotateSpeed /= 1000;

        //If the radius is not set, or incorrect, it is set to the distance from the center of the first object
        if(radius <= 0)
        {
            radius = Vector2.Distance(gameObject.transform.position, platforms[0].transform.position);
        }
    }

    void FixedUpdate()
    {
        //If game isn't paused, rotate
        if (menu.GetShowing().Equals("none"))
        {
            //Determines where the circle will start based on the rotate speed, allows for the rotation of the circle
            startingAngle += rotateSpeed;

            //Sets the angle back to zero after a full rotation, prevents unnecessarily high numbers 
            if (startingAngle >= 2 * Mathf.PI)
            {
                startingAngle -= 2 * Mathf.PI;
            }

            Vector2[] points = new Vector2[platforms.Length];

            for (int i = 0; i < points.Length; i++)
            {
                //Establishes the angle each object will be at, then calculates its relative x and y position and sets it in the points array
                float angle = (2 * Mathf.PI * i / platforms.Length) + startingAngle;

                float x = radius * Mathf.Cos(angle);
                float y = radius * Mathf.Sin(angle);

                points[i] = new Vector2(x, y);
            }

            //Sets the actual position of each object
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].transform.localPosition = points[i];
            }
        }
    }
}
