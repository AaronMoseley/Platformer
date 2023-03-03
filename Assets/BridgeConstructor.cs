using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeConstructor : MonoBehaviour
{
    public Vector2 startPos;
    public int bridgeHeight;
    public int bridgeLength;
    public int sideTaper;
    public GameObject block;

    public GameObject[][] blocks;

    void Start()
    {
        blocks = new GameObject[bridgeHeight + 1][];

        for(int i = 0; i < blocks.Length; i++)
        {
            float y = startPos.y - i;
            blocks[i] = new GameObject[bridgeLength];

            if (i < bridgeHeight)
            {
                for (int j = 0; j < bridgeLength; j++)
                {
                    float x = startPos.x + j;

                    GameObject temp = Instantiate(block, new Vector2(x, y), Quaternion.Euler(Vector3.zero));
                    temp.transform.SetParent(gameObject.transform);
                    temp.GetComponent<BridgeBlock>().SetArrayPos(i, j);
                    blocks[i][j] = temp;
                }
            }
        }

        for(int i = 0; i < sideTaper * 2; i++)
        {
            float y = startPos.y - bridgeHeight;
            float x;

            if (i < sideTaper)
            {
                x = startPos.x + i;
            } else
            {
                x = startPos.x + bridgeLength - i + (sideTaper - 1);
            }

            GameObject temp = Instantiate(block, new Vector2(x, y), Quaternion.Euler(Vector3.zero));
            temp.transform.SetParent(gameObject.transform);
            temp.GetComponent<BridgeBlock>().SetArrayPos(bridgeHeight, i);
            blocks[bridgeHeight][i] = temp;
        }
    }
}
