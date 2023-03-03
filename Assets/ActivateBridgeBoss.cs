using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBridgeBoss : MonoBehaviour
{
    public GameObject boss;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            boss.GetComponent<BridgeBoss>().SetState("searching");
        }
    }
}
