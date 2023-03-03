using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData : MonoBehaviour
{
    //Allows the player to carry over settings from one scene to another

    [Header("Options List")]
    public Resolution resolution;
    public float volume;
    public bool fullscreen;
    public int quality;
    public string[][] keyBindings;
    [Space]

    InputManager input;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (GameObject.FindGameObjectsWithTag("Stored Settings").Length > 1)
        {
            Destroy(gameObject);
        }

        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();

        //Establishes the keybinding string double-arrays
        keyBindings = new string[input.buttons.Count][];

        for (int i = 0; i < keyBindings.Length; i++)
        {
            keyBindings[i] = new string[2];
        }
    }

    public void UpdateBindings()
    {
        for (int i = 0; i < input.buttons.Count; i++)
        {
            //Updates the keybinding double array to what the input manager has
            keyBindings[i][0] = input.buttons[i].name;
            keyBindings[i][1] = input.buttons[i].code.ToString();
        }
    }
}
