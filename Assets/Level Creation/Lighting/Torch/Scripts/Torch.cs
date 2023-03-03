using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    //Alternates the torch object through various sprites to animate it, also moves the smoke sprite for animation
    
    [Header("Art")]
    public Sprite[] frames;
    public Sprite smoke;
    [Space]

    [Header("Animation Information")]
    public GameObject smokeSprite;
    [Space]
    public float animSpeed;
    public float smokeAnimSpeed;
    [Space]

    int currFrame = 0;

    float animTimer;
    float smokeTimer;

    void Update()
    {
        animTimer += Time.deltaTime;
        smokeTimer += Time.deltaTime;

        //If enough time has passed to switch frames, advance to the next frame
        if(animTimer >= animSpeed)
        {
            animTimer = 0;

            //When the frame reaches the end of the list, reset the current fram to the beginning
            if(currFrame + 1 >= frames.Length)
            {
                currFrame = -1;
            }

            currFrame++;
            gameObject.GetComponent<SpriteRenderer>().sprite = frames[currFrame];
        }

        //If enough time has passed to animate the smoke, reverse the x scale
        if(smokeTimer >= smokeAnimSpeed)
        {
            smokeTimer = 0;

            smokeSprite.transform.localScale = new Vector2(smokeSprite.transform.localScale.x * -1, smokeSprite.transform.localScale.y);
        }
    }
}
