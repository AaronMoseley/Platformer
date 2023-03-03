using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class PlayerFootCollider : MonoBehaviour
{
    //Controls the audio and particle system corresponding to the player's feet

    [Header("Audio")]
    public AudioSource jumpEffect;
    [Space]

    [Header("Particle Effects")]
    public int numParticles;
    public Tilemap foreground;
    [Space]

    ParticleSystem particles;
    Rigidbody2D playerRB;

    void Start()
    {
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the player lands on the ground or a platform and their feet are not already on something
        if ((collision.gameObject == foreground.gameObject || collision.gameObject.CompareTag("Moving Platform")) && gameObject.GetComponent<Collisions>().collidingObject == null)
        {
            //Play the landing audio
            jumpEffect.Play();

            //Finds the sprite of the object
            Sprite tileSprite;

            bool temp = true;
            if (collision.gameObject.GetComponent<Tilemap>())
            {
                //Finds the location of the tile and the sprite on that tile
                Tilemap collidingTilemap = collision.gameObject.GetComponent<Tilemap>();

                RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.down);
                float yDiff = (int)(gameObject.transform.position.y - hit.point.y) + 1;

                if (collidingTilemap.GetTile<Tile>(Vector3Int.FloorToInt(gameObject.transform.position - new Vector3(0, yDiff, 0))))
                {
                    tileSprite = collidingTilemap.GetTile<Tile>(Vector3Int.FloorToInt(gameObject.transform.position - new Vector3(0, yDiff, 0))).sprite;
                } else
                {
                    tileSprite = null;
                    temp = false;
                }
            } else
            {
                tileSprite = collision.gameObject.GetComponent<SpriteRenderer>().sprite;
            }

            if (temp)
            {
                //Finds the position of the sprite in the texture
                Texture2D tileTexture = tileSprite.texture;

                Vector2Int spriteOffset = new Vector2Int((int)tileSprite.rect.x, (int)tileSprite.rect.y);

                //Sets the color of the particles and plays it
                var main = particles.main;

                main.startColor = tileTexture.GetPixel(spriteOffset.x, spriteOffset.y);
                main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 1);

                particles.Play();
            }
        }
    }
}
