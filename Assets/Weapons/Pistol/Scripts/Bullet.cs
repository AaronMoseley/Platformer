using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Controls the bullet as it moves toward the player and its damage

    [Header("Movement Information")]
    public float bulletSpeed;
    public float timeAlive;
    float bulletLifeTimer;
    [Space]

    [Header("Collision Information")]
    public int hookLayer;
    [Space]

    [Header("Audio")]
    public AudioSource impactSound;
    [Space]

    GameObject player;
    Rigidbody2D rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        //Sets the velocity of the bullet
        rb.velocity = new Vector2(bulletSpeed * Mathf.Cos(Mathf.Deg2Rad * gameObject.transform.localEulerAngles.z), bulletSpeed * Mathf.Sin(Mathf.Deg2Rad * gameObject.transform.localEulerAngles.z));
    }

    void Update()
    {
        //If the bullet has been alive too long, destroy it
        bulletLifeTimer += Time.deltaTime;

        if(bulletLifeTimer >= timeAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the bullet hits the player, kill the player and destroy this
        if(collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().TakeDamage();
            Destroy(gameObject);
        } else if (!collision.collider.isTrigger && collision.gameObject.layer != hookLayer)
        {
            //Destroy the bullet if it hits anything else
            StartCoroutine(BulletHit());
        }
    }

    IEnumerator BulletHit()
    {
        //Play the audio, disable any colliders/art, then destroy the bullet after the audio finishes
        impactSound.Play();
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitWhile(() => impactSound.isPlaying);
        Destroy(gameObject);
    }
}
