using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBlock : MonoBehaviour
{
    public Sprite topCrackedBricks;
    public Sprite bottomCrackedBricks;

    public Sprite topBricks;
    public Sprite bottomBricks;

    public Vector2Int arrayPos;

    public int destroyWaitTime;

    BridgeConstructor constructor;

    string state = "solid";

    void Start()
    {
        constructor = gameObject.GetComponentInParent<BridgeConstructor>();
    }

    void Update()
    {
        
    }

    public void Crack()
    {
        if (arrayPos.x < constructor.blocks.Length - 1)
        {
            if (constructor.blocks[arrayPos.x + 1][arrayPos.y] != null)
            {
                constructor.blocks[arrayPos.x + 1][arrayPos.y].GetComponent<BridgeBlock>().Crack();
            }
        }

        if (arrayPos.x == 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = topCrackedBricks;
        } else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = bottomCrackedBricks;
        }

        state = "cracked";
    }

    public void Break()
    {
        gameObject.transform.SetParent(null);

        gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;

        if (arrayPos.x < constructor.blocks.Length - 1)
        {
            if (constructor.blocks[arrayPos.x + 1][arrayPos.y] != null)
            {
                constructor.blocks[arrayPos.x + 1][arrayPos.y].GetComponent<BridgeBlock>().Break();
            }
        }

        BoxCollider2D[] colliders = gameObject.GetComponents<BoxCollider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].usedByComposite = false;
            colliders[i].isTrigger = true;
        }

        StartCoroutine(KillBlock());

        state = "broken";
    }

    public void SetArrayPos(int x, int y)
    {
        arrayPos = new Vector2Int(x, y);

        if (arrayPos.x == 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = topBricks;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = bottomBricks;
        }
    }

    IEnumerator KillBlock()
    {
        yield return new WaitForSeconds(destroyWaitTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            if (collision.gameObject.GetComponent<BridgeBoss>().GetState().Equals("stationary") || collision.gameObject.GetComponent<BridgeBoss>().GetState().Equals("slamming"))
            {
                switch (state)
                {
                    case "solid":
                        Crack();
                        break;
                    case "cracked":
                        Break();
                        break;
                }
            }
        }
    }
}
