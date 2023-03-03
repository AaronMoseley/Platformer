using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunManager : MonoBehaviour
{
    //Allows the enemy to shoot at the player and maneuver the gun 

    [Header("Art")]
    public GameObject enemyGFX;
    [Space]
    
    [Header("Shooting Information")]
    public GameObject barrel;
    public GameObject bullet;
    public float timeBetweenShots;
    [Space]

    [Header("Audio")]
    public AudioSource gunshot;
    [Space]

    [Header("Collision Information")]
    public int groundLayer;
    [Space]

    bool colliding = false;
    float correctAngle;
    float shotTimer;

    EnemyMovement movement;
    GameObject enemy;
    GameObject player;

    void Start()
    {
        movement = gameObject.GetComponentInParent<EnemyMovement>();
        enemy = movement.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");

        //This needs to be at the max so the enemy can shoot whenever they see the player
        shotTimer = timeBetweenShots;
    }

    void Update()
    {
        //Finds the angle of rotation that can be used in-engine
        if(gameObject.transform.localRotation.eulerAngles.z > 180)
        {
            correctAngle = gameObject.transform.localRotation.eulerAngles.z - 360;
        } else
        {
            correctAngle = gameObject.transform.localRotation.eulerAngles.z;
        }

        //Sets the y-scale so the gun isn't upside down when rotating
        if (Mathf.Abs(correctAngle) > 90)
        {
            gameObject.transform.localScale = new Vector3(1, -1, 1);
            enemyGFX.transform.localScale = new Vector3(-1, 1, 1);
        } else
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            enemyGFX.transform.localScale = new Vector3(1, 1, 1);
        }

        if(movement.GetDetectionState().Equals("shooting"))
        {
            //If this enemy is shooting, rotate the gun to the correct angle based on where the player is and increase the shot timer
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y - enemy.transform.position.y, player.transform.position.x - enemy.transform.position.x)));
            shotTimer += Time.deltaTime;
        } else
        {
            //If this enemy is not shooting, set the gun to directly in front of it
            if(movement.GetDirection() == -1 && gameObject.transform.localRotation.eulerAngles.z != 180)
            {
                gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
            } else if(movement.GetDirection() == 1 && gameObject.transform.localRotation.eulerAngles.z != 0)
            {
                gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        //Shoot if the timer is greater than the time between shots
        if (shotTimer >= timeBetweenShots && movement.GetDetectionState().Equals("shooting") && !colliding)
        {
            Shoot();
            shotTimer = 0;
        }
    }

    public void Shoot()
    {
        //Play the gunshot audio and create the bullet at the barrel
        gunshot.Play();
        Instantiate(bullet, barrel.transform.position, gameObject.transform.localRotation);
    }

    //Logs when the gun is inside the ground and therefore cannot shoot
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = false;
        }
    }
}
