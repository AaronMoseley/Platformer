using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    //Controls how the rotation and scale of the tool the player is holding

    [Header("Gun/Grappler Settings")]
    public Grappler grappler;
    public float xOffset;
    public float yOffset;
    [Space]

    bool equipped = false;
    bool hasGrappler = false;

    InputManager input;
    GameObject player;

    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        if(grappler)
        {
            hasGrappler = true;
        }
    }

    void Update()
    {
        //Checks if the player has a grappler
        if (hasGrappler)
        {
            //Allows the player to equip the grappler
            if (input.ButtonDown("Equip Grappler") && !equipped)
            {
                grappler.gameObject.SetActive(true);
                equipped = true;
            }
            else if (input.ButtonDown("Equip Grappler"))
            {
                //Unequipping the grappler and resetting its variables
                grappler.SetState("stationary");

                grappler.hook.GetComponent<Rigidbody2D>().isKinematic = true;
                grappler.hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                grappler.hook.transform.SetParent(gameObject.transform);
                grappler.hook.transform.localPosition = grappler.hookStartPos;
                grappler.hook.transform.localRotation = Quaternion.Euler(Vector3.zero);
                grappler.hook.GetComponent<Hook>().edgeCollider.enabled = false;
                grappler.hook.GetComponent<Hook>().radius = 0;

                grappler.gameObject.SetActive(false);
                equipped = false;
            }

            //Finds the correct angle in the engine
            float correctAngle;

            if (gameObject.transform.localRotation.eulerAngles.z > 180)
            {
                correctAngle = gameObject.transform.localRotation.eulerAngles.z - 360;
            }
            else
            {
                correctAngle = gameObject.transform.localRotation.eulerAngles.z;
            }

            //Modifies the scale depending on where the rotation is
            if (Mathf.Abs(correctAngle) > 90)
            {
                gameObject.transform.localScale = new Vector3(1, -1, 1);
            }
            else if (Mathf.Abs(correctAngle) < 90)
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }

            if ((grappler.GetState() == "landed" || grappler.GetState() == "pulling"))
            {
                //Rotate the grappler towards the hook if it's landed or pulling
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(grappler.hook.transform.position.y - player.transform.position.y, grappler.hook.transform.position.x - player.transform.position.x)));
            }
            else
            {
                //Rotates the grappler towards the mouse otherwise
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x)));
            }
        }
    }

    public void SetGrappler(Grappler newGrappler)
    {
        grappler = newGrappler;
        hasGrappler = true;
        equipped = true;
    }
}
