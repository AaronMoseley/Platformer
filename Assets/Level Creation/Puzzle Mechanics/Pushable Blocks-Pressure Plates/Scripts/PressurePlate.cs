using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    //Moves a door to a specified offset when the player or a moveable block collides with this object

    [Header("Art")]
    public Sprite raisedImage;
    public Sprite pressedImage;
    [Space]

    [Header("Activated Door")]
    public PuzzleDoor[] connectedDoors;
    [Space]

    List<GameObject> pressedBy = new List<GameObject>();

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Press(bool state, GameObject collision)
    {
        //Visually displays whether the plate is being pressed and opens/closes the door
        if(state)
        {
            spriteRenderer.sprite = pressedImage;
            pressedBy.Add(collision.gameObject);

            for (int i = 0; i < connectedDoors.Length; i++)
            {
                connectedDoors[i].Open(true);
            }
        } else
        {
            spriteRenderer.sprite = raisedImage;

            for (int i = 0; i < connectedDoors.Length; i++)
            {
                connectedDoors[i].Open(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the player or a block collide with this, press down the plate
        if((collision.CompareTag("Player") || collision.CompareTag("Moveable Block")))
        {
            Press(true, collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Figures out whether the player/moveable block have left
        int temp = -1;

        for(int i = 0; i < pressedBy.Count; i++)
        {
            if(pressedBy[i] == collision.gameObject)
            {
                temp = i;
            }
        }

        //If an object has left, remove it from the Pressed-By list and close the door if nothing is pressing
        if (temp != -1)
        {
            pressedBy.RemoveAt(temp);

            if (pressedBy.Count == 0)
            {
                Press(false, null);
            }
        }
    }
}
