using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    //Stores the key that the player is currently holding and is able to destroy it

    public float destroyKeyWaitTime;

    GameObject currKey;

    public GameObject GetCurrKey()
    {
        return currKey;
    }

    public void SetCurrKey(GameObject nextKey)
    {
        currKey = nextKey;
    }

    //Destroys key after certain amount of time
    public IEnumerator DestroyKey(GameObject key)
    {
        yield return new WaitForSeconds(destroyKeyWaitTime);
        Destroy(key);
    }
}
