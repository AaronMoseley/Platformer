using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PlayerSaveSystem : MonoBehaviour
{
    //Allows the player to save and load their progress
    
    [Header("Save Settings")]
    public float waitTime;
    [Space]

    [Header("UI Elements")]
    public GameObject overrideMenu;
    public InGameMenuManager menu;
    [Space]

    [Header("Load Prefabs")]
    public GameObject grapplerPrefab;
    [Space]

    bool loading = false;
    int correctScene;
    int currOverriding;

    GameObject player;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        correctScene = SceneManager.GetActiveScene().buildIndex;
        player = GameObject.FindGameObjectWithTag("Player");
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
    }

    void Update()
    {
        //Failsafe if this object didn't get destroyed correctly
        if(SceneManager.GetActiveScene().buildIndex != correctScene && !loading)
        {
            Destroy(gameObject);
        }
    }

    public void TestCanSave(int saveNum)
    {
        if(PlayerSaveLoad.LoadPlayer(saveNum) == null)
        {
            //Lets the player save if there's no save file
            Save(saveNum);
            currOverriding = -1;
        } else
        {
            //Opens the override menu if there's already a save file
            overrideMenu.SetActive(true);
            currOverriding = saveNum;
        }
    }

    public void Save(int saveNum)
    {
        //Resets the UI elements
        overrideMenu.SetActive(false);
        menu.ShowSaves(false);
        
        if (saveNum == -1)
        {
            saveNum = currOverriding;
        }
        
        currOverriding = -1;

        //Failsafe if the player wasn't identified correctly
        player = GameObject.FindGameObjectWithTag("Player");

        bool temp = player.GetComponentInChildren<GunManager>().gameObject.GetComponentInChildren<Grappler>();

        //Saves the player's position and scene
        PlayerSaveLoad.SaveData(player.transform.position, SceneManager.GetActiveScene().buildIndex, saveNum, temp);
    }

    public void Load(int saveNum)
    {
        //Gets the specified save file and loads the scene
        Time.timeScale = 1;
        PlayerData data = PlayerSaveLoad.LoadPlayer(saveNum);

        loading = true;
        SceneManager.LoadScene(data.currScene);

        StartCoroutine(LoadPlayer(data));
    }

    public void SetOverriding(int newOverriding)
    {
        currOverriding = newOverriding;
    }

    IEnumerator LoadPlayer(PlayerData data)
    {
        //Waits to adjust the player's position because if it doesn't wait, it doesn't work, then destroys this object
        yield return new WaitForSeconds(waitTime);
        player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = new Vector3(data.xPos, data.yPos, data.zPos);

        if(data.grappler && !player.GetComponentInChildren<GunManager>().gameObject.GetComponentInChildren<Grappler>())
        {
            GameObject temp = Instantiate(grapplerPrefab, player.GetComponentInChildren<GunManager>().gameObject.transform);
            player.GetComponentInChildren<GunManager>().SetGrappler(temp.GetComponent<Grappler>());
            player.GetComponent<Movement>().SetUpGrappler(true);
        } else if(!data.grappler && player.GetComponentInChildren<GunManager>().gameObject.GetComponentInChildren<Grappler>())
        {
            Destroy(player.GetComponentInChildren<GunManager>().gameObject.GetComponentInChildren<Grappler>().gameObject);
        }

        Destroy(gameObject);
    }
}
