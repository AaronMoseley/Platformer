using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    //Controls all of the menus within gameplay

    [Header("Menu Objects")]
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject bindingMenu;
    public GameObject saveMenu;
    public GameObject loadMenu;
    public GameObject overrideMenu;
    public GameObject deathMenu;
    [Space]

    [Header("Options Components")]
    public Dropdown resDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;
    public AudioMixer audioMix;
    Resolution[] resolutions;
    [Space]

    [Header("Scene Settings")]
    public int mainMenuSceneNum = 0;
    [Space]

    string showing = "none";

    InputManager input;

    GameObject player;

    SettingsData storedSettings;
    PlayerSaveSystem saveSystem;

    void Start()
    {
        input = gameObject.GetComponent<InputManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        storedSettings = GameObject.FindGameObjectWithTag("Stored Settings").GetComponent<SettingsData>();
        saveSystem = GameObject.FindGameObjectWithTag("Save System").GetComponent<PlayerSaveSystem>();

        if(SaveSettingsProcess.LoadSettings() != null)
        {
            //If there's already a list of settings saved, load them in the stored settings script
            SavedSettings data = SaveSettingsProcess.LoadSettings();

            storedSettings.resolution.height = data.resHeight;
            storedSettings.resolution.width = data.resWidth;
            storedSettings.fullscreen = data.fullscreen;
            storedSettings.quality = data.quality;
            storedSettings.volume = data.volume;
            storedSettings.keyBindings = data.bindings;

            //Update the key bindings of the input buttons to match the settings file
            for(int i = 0; i < input.buttons.Count; i++)
            {
                for (int j = 0; j < data.bindings.Length; j++)
                {
                    if (input.buttons[i].name == data.bindings[j][0])
                    {
                        input.buttons[i].code = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.bindings[i][1]);
                    }
                }
            }
        } else
        {
            //If there's not a save file, set the stored settings to the default monitor resolution
            storedSettings.resolution.height = Screen.currentResolution.height;
            storedSettings.resolution.width = Screen.currentResolution.width;
        }

        //Creates a list of the possible resolutions for the monitor
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> resolutionStrings = new List<string>();

        int currResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            //Updates the dropdown with the possible resolutions
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionStrings.Add(option);

            //Sets the dropdown to the current resolution
            if (resolutions[i].width == storedSettings.resolution.width && resolutions[i].height == storedSettings.resolution.height)
            {
                currResIndex = i;
            }
        }

        //Sets all of the parts on the options menu to the correct values
        resDropdown.AddOptions(resolutionStrings);
        resDropdown.value = currResIndex;
        resDropdown.RefreshShownValue();

        volumeSlider.value = storedSettings.volume;
        fullscreenToggle.isOn = storedSettings.fullscreen;
        qualityDropdown.value = storedSettings.quality;
    }

    void Update()
    {
        if(input.ButtonDown("Cancel") && showing.Equals("none") && !player.GetComponent<Movement>().talking)
        {
            //If the player presses pause and nothing is showing, pause the game
            Pause(false);
        } else if(input.ButtonDown("Cancel") && showing.Equals("pause"))
        {
            //If the game is paused and the player presses cancel, unpause the game
            Pause(true);
        } else if(input.Button("Cancel") && showing.Equals("loads"))
        {
            //If the game is showing the load screen and the player presses cancel, show the pause screen
            loadMenu.SetActive(false);
            showing = "pause";
        }

        if(input.ButtonDown("Cancel") && showing.Equals("options"))
        {
            //If the player presses cancel and the options are showing, show the pause menu
            pauseMenu.SetActive(true);
            bindingMenu.SetActive(false);
            optionsMenu.SetActive(false);

            SaveSettingsProcess.SaveSettings(storedSettings);

            showing = "pause";
        } else if(input.ButtonDown("Cancel") && showing.Equals("bindings"))
        {
            //If the player presses cancel and the bindings are showing, show the options menu
            bindingMenu.SetActive(false);
            optionsMenu.SetActive(false);
            optionsMenu.SetActive(true);

            storedSettings.UpdateBindings();

            showing = "options";
        }
    }

    //Scene Management

    public void BackToMenu()
    {
        //Returns to the main menu
        SceneManager.LoadScene(mainMenuSceneNum);
    }

    //Menu Management

    public void Pause(bool state)
    {
        //Sets some player components to the state variable and either activates or deactivates the pause menu
        player.GetComponent<Movement>().enabled = state;
        player.GetComponentInChildren<GunManager>().enabled = state;
        pauseMenu.SetActive(!state);

        //Changes the time scale and showing variable based on whether the game is being paused
        if (!state)
        {
            Time.timeScale = 0;
            showing = "pause";
        }
        else
        {
            Time.timeScale = 1;
            showing = "none";
        }
    }

    public void LoadOptions()
    {
        //Shows the options menu
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        bindingMenu.SetActive(false);
        showing = "options";
    }

    public void ShowBindings()
    {
        //Shows the binding menu
        bindingMenu.SetActive(true);
        pauseMenu.SetActive(false);
        showing = "bindings";
    }

    public void ShowSaves(bool state)
    {
        //Shows the save menu if the state variable is true
        saveMenu.SetActive(state);

        if (state)
        {
            //Pauses the game and updates the text by the save files
            showing = "saves";
            player.GetComponent<Movement>().enabled = false;
            player.GetComponentInChildren<GunManager>().enabled = false;
            Time.timeScale = 0;

            LoadInfoDisplay[] temp = saveMenu.GetComponentsInChildren<LoadInfoDisplay>();

            for(int i = 0; i < temp.Length; i++)
            {
                temp[i].UpdateText();
            }
        } else
        {
            //Gets back to the normal game
            showing = "none";
            player.GetComponent<Movement>().enabled = true;
            player.GetComponentInChildren<GunManager>().enabled = true;
            Time.timeScale = 1;
        }
    }

    public void CloseOverrideMenu()
    {
        //Closes the override menu and updates the save system overriding system
        overrideMenu.SetActive(false);
        saveSystem.SetOverriding(-1);
    }

    public void ShowLoad()
    {
        //Shows the load menu
        loadMenu.SetActive(true);
        showing = "loads";
    }

    public void PlayerDie()
    {
        //When the player dies, enable the death menu
        pauseMenu.SetActive(false);
        deathMenu.SetActive(true);
    }

    //Options Management

    public void SetVolume(float volume)
    {
        //Sets the volume when the slider is adjusted
        storedSettings.volume = volume;
        audioMix.SetFloat("MasterVolume", volume);
    }

    public void SetQuality(int quality)
    {
        //Sets the quality when the dropdown is changed
        storedSettings.quality = quality;
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        //Sets the fullscreen variable when the toggle is clicked
        storedSettings.fullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resIndex)
    {
        //Changes the resolution when the dropdown is changed
        Resolution res = resolutions[resIndex];
        storedSettings.resolution = res;
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public string GetShowing()
    {
        return showing;
    }

    public void SetShowing(string newShowing)
    {
        showing = newShowing;
    }
}
