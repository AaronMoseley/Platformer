using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGlobalLight : MonoBehaviour
{
    public float newIntensity;
    float lastIntensity = -1;

    public int forwardDir = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (lastIntensity == -1)
            {
                lastIntensity = gameObject.GetComponentInParent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity;
            }

            if(Mathf.Sign(gameObject.transform.position.x - collision.transform.position.x) == Mathf.Sign(forwardDir))
            {
                gameObject.GetComponentInParent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = newIntensity;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && Mathf.Sign(gameObject.transform.position.x - collision.transform.position.x) == Mathf.Sign(forwardDir))
        {
            gameObject.GetComponentInParent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = lastIntensity;
        }
    }
}
