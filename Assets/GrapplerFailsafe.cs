using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerFailsafe : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.isTrigger && player.GetComponentInChildren<GunManager>().gameObject.GetComponentInChildren<Grappler>() && !collision.gameObject.CompareTag("Player"))
        {
            player.GetComponentInChildren<GunManager>().gameObject.GetComponentInChildren<Grappler>().Retract();
        }
    }
}
