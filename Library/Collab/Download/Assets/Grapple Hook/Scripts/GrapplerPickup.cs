using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GrapplerPickup : MonoBehaviour
{
    public string pickupMessage = "Press USE to pick up the grappling hook";
    public GameObject grapplerPrefab;

    bool canPickUp = false;

    GameObject gunManager;
    Text pickupText;
    InputManager input;

    void Start()
    {
        gunManager = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<GunManager>().gameObject;
        pickupText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
    }

    void Update()
    {
        if(canPickUp && input.ButtonDown("Use"))
        {
            GameObject temp = Instantiate(grapplerPrefab, gunManager.transform);
            gunManager.GetComponent<GunManager>().SetGrappler(temp.GetComponent<Grappler>());
            gunManager.GetComponentInParent<Movement>().SetUpGrappler(true);

            pickupText.text = "";
            pickupText.enabled = false;

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            pickupText.text = pickupMessage;
            pickupText.enabled = true;
            canPickUp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("asd");
        if (collision.gameObject.CompareTag("Player") && canPickUp)
        {
            pickupText.text = "";
            pickupText.enabled = false;
            canPickUp = false;
        }
    }
}
