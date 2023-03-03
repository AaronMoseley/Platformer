using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    GameObject player;
    Inventory inventory;

    public float xOffset;
    public float yOffset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = GameObject.FindGameObjectWithTag("Inventory Manager").GetComponent<Inventory>();
    }

    void Update()
    {
        float correctAngle;

        if (gameObject.transform.localRotation.eulerAngles.z > 180)
        {
            correctAngle = gameObject.transform.localRotation.eulerAngles.z - 360;
        }
        else
        {
            correctAngle = gameObject.transform.localRotation.eulerAngles.z;
        }

        if (Mathf.Abs(correctAngle) > 90)
        {
            gameObject.transform.localScale = new Vector3(1, -1, 1);
        }
        else if (Mathf.Abs(correctAngle) < 90)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        if (gameObject.GetComponentInChildren<Grappler>() && !inventory.invOpen)
        {
            Grappler grappler = gameObject.GetComponentInChildren<Grappler>();
            if((grappler.state == "landed" || grappler.state == "pulling"))
            {
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(grappler.hook.transform.position.y - player.transform.position.y, grappler.hook.transform.position.x - player.transform.position.x)));
            } else
            {
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x)));
            }
        } else if(!inventory.invOpen)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - player.transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - player.transform.position.x)));
        }
    }
}
