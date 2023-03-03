using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBoss : MonoBehaviour
{
    public float moveSpeed;
    public float slamSpeed;
    public float slamWaitTime;
    public float riseSpeed;
    public float sweepSpeed;
    public float postSlamWaitTime;
    public float width;
    public float height;
    public int groundLayer = 8;

    public GameObject key;
    public GameObject lockedDoor;
    public Vector2 keyInstantPos;

    public GameObject bridgeConstructor;
    public int destroyYDiff;

    float startYPos;
    public int dir = -1;
    public string state = "stationary";

    void Start()
    {
        startYPos = gameObject.transform.position.y;
    }

    void Update()
    {
        if(gameObject.transform.position.y <= bridgeConstructor.GetComponent<BridgeConstructor>().startPos.y - destroyYDiff)
        {
            GameObject newKey = Instantiate(key, keyInstantPos, Quaternion.Euler(Vector3.zero));
            lockedDoor.GetComponent<LockedDoor>().key = newKey;
            Destroy(gameObject);
        }

        if (!state.Equals("stationary"))
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + (height / 2)), new Vector2(dir, 0), Mathf.Infinity, LayerMask.GetMask("Ground"));

            if (Vector2.Distance(gameObject.transform.position, hit.point) <= width / 2)
            {
                switch (state)
                {
                    case "searching":
                        int temp = Random.Range(0, 2);

                        if (temp == 0)
                        {
                            if (gameObject.transform.position.y >= startYPos)
                            {
                                dir *= -1;
                            }
                            else
                            {
                                state = "rising";
                            }
                        }
                        else
                        {
                            dir *= -1;
                            state = "slam-sweeping";
                        }
                        break;

                    case "sweeping":
                        state = "rising";
                        dir *= -1;
                        break;
                }
            }

            switch (state)
            {
                case "searching":
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(dir * moveSpeed, 0);

                    RaycastHit2D playerDetect = Physics2D.Raycast(gameObject.transform.position, Vector2.down);

                    if (playerDetect.rigidbody.gameObject.CompareTag("Player"))
                    {
                        state = "stationary";
                        StartCoroutine(Slam());
                    }

                    break;

                case "slamming":
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -slamSpeed);
                    break;

                case "slam-sweeping":
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -slamSpeed);
                    break;

                case "rising":
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, riseSpeed);

                    if (gameObject.transform.position.y >= startYPos)
                    {
                        state = "searching";
                    }
                    break;

                case "sweeping":
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(dir * sweepSpeed, 0);
                    break;
            }
        }
    }

    IEnumerator Slam()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(slamWaitTime);
        state = "slamming";
    }

    IEnumerator Rise()
    {
        state = "stationary";
        yield return new WaitForSeconds(postSlamWaitTime);
        state = "rising";
    }

    public string GetState()
    {
        return state;
    }

    public void SetState(string newState)
    {
        state = newState;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            state = "stationary";
        }

        if (collision.gameObject.GetComponent<BridgeConstructor>())
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            switch (state)
            {
                case "slamming":
                    StartCoroutine(Rise());
                    break;

                case "slam-sweeping":
                    state = "sweeping";
                    break;
            }
        }
    }
}
