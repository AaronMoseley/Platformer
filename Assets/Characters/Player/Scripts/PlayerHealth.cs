using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //Controls the player's dying system and lets them take damage

    [Header("Audio")]
    public AudioSource damagedSound;
    [Space]

    [Header("Death Information")]
    public float deathWaitTime;
    public GameObject deathParticles;
    [Space]

    GameObject gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
    }

    public void TakeDamage()
    {
        //Plays the death sound, displays the blood particles, and disables multiple parts of the player
        damagedSound.Play();
        deathParticles.SetActive(true);

        gameManager.GetComponent<InGameMenuManager>().PlayerDie();
        gameObject.GetComponent<Movement>().enabled = false;

        if (gameObject.GetComponentInChildren<Grappler>())
        {
            gameObject.GetComponentInChildren<Grappler>().hook.SetActive(false);
        }

        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //Waits until the death sound finishes, then disables all the children of the player
        yield return new WaitWhile(() => damagedSound.isPlaying);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject currObject = gameObject.transform.GetChild(i).gameObject;
            if (!currObject.GetComponent<SpriteRenderer>() && !currObject.GetComponentInChildren<ParticleSystem>())
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
