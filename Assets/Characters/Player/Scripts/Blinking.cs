using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour
{
    //Allows any character to blink with a minimum of two sprites
    
    [Header("Art")]
    public Sprite normalCharacter;
    public Sprite characterBlink;
    public Sprite crouchedCharacter;
    public Sprite crouchCharacterBlink;
    [Space]

    [Header("Blinking Information")]
    public float timeBetweenBlinks;
    public float blinkTime;
    bool blinking;
    float blinkTimer;
    [Space]

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        blinkTimer += Time.deltaTime;

        if (!blinking && blinkTimer >= timeBetweenBlinks)
        {
            //If enough time has passed, blink
            blinking = true;
            blinkTimer = 0;

            //If the current sprite is the normal character, switch to the blinking character, otherwise go to the crouch-blink image
            if (spriteRenderer.sprite == normalCharacter)
            {
                spriteRenderer.sprite = characterBlink;
            }
            else
            {
                spriteRenderer.sprite = crouchCharacterBlink;
            }
        }
        else if (blinking && blinkTimer >= blinkTime)
        {
            //If the character is currently blinking but shouldn't be, go back to the normal sprites
            blinking = false;
            blinkTimer = 0;

            if (spriteRenderer.sprite == characterBlink)
            {
                spriteRenderer.sprite = normalCharacter;
            }
            else
            {
                spriteRenderer.sprite = crouchedCharacter;
            }
        }
    }
}
