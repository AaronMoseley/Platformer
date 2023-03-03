using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolCurves : MonoBehaviour
{
    //Makes the audio effect of this object react to the distance of the player

    [Header("Audio Listener (Player if not set)")]
    public Transform target;
    [Space]

    [Header("Base Volume of Effect")]
    public float curveScale;
    [Space]

    AudioSource source;

    void Start()
    {
        //Finds the audio source attached to this object and sets the Player as the audio listener if nothing has been set
        source = gameObject.GetComponent<AudioSource>();

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        //Modifies the volume of the effect based on the curve scale set in the editor and the distance of this object from the listener
        float volume = curveScale / Vector2.Distance(target.position, gameObject.transform.position);
        source.volume = Mathf.Clamp(volume, 0, 1);
    }
}
