using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //Controls the main menu system

    [Header("Menu Settings")]
    public GameObject bindingMenu;
    public GameObject saveMenu;
    public Vector3 optionsPosition;
    public float loadSpeed;
    public float distError;
    Vector3 normalPosition;
    Vector3 targetPos;
    bool loadingOptions;
    [Space]

    [Header("Options Components")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;
    public AudioMixer audioMix;
    public Dropdown resDropdown;
    Resolution[] resolutions;
    List<Resolution> usableResolutions = new List<Resolution>();
    [Space]

    [Header("Misc Settings")]
    public Button loadSaveButton;
    public Button[] saveSlots;
    public int firstLevel;
    public int numSaves;
    [Space]

    bool showingBindings = false;

    GameObject mainCamera;
    InputManager input;

    SettingsData storedSettings;
    PlayerSaveSystem saveSystem;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        input = gameObject.GetComponent<InputManager>();

        storedSettings = GameObject.FindGameObjectWithTag("Stored Settings").GetComponent<SettingsData>();
        saveSystem = GameObject.FindGameObjectWithTag("Save System").GetComponent<PlayerSaveSystem>();

        normalPosition = mainCamera.transform.position;
        Time.timeScale = 1;


        if (SaveSettingsProcess.LoadSettings() != null)
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
            for (int i = 0; i < input.buttons.Count; i++)
            {
                input.buttons[i].name = data.bindings[i][0];
                input.buttons[i].code = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.bindings[i][1]);
            }
        } else
        {
            //If there's not a save file, set the stored settings to the default monitor resolution
            storedSettings.resolution.height = Screen.currentResolution.height;
            storedSettings.resolution.width = Screen.currentResolution.width;
        }

        //If the player has saves, this sets the buttons' interactibility to the correct state
        bool hasSave = false;

        for(int i = 0; i < numSaves; i++)
        {
            if(PlayerSaveLoad.LoadPlayer(i + 1) != null)
            {
                hasSave = true;
            } else
            {
                saveSlots[i].interactable = false;
            }
        }

        if(!hasSave)
        {
            loadSaveButton.interactable = false;
        }
        
        //Finds the correct resolution and updates the dropdown to reflect that
        int screenRefreshRate = Screen.currentResolution.refreshRate;

        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> resolutionStrings = new List<string>();

        int currResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == screenRefreshRate || resolutions[i].refreshRate == screenRefreshRate - 1)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                resolutionStrings.Add(option);
                usableResolutions.Add(resolutions[i]);
            }

            if (resolutions[i].width == storedSettings.resolution.width && resolutions[i].height == storedSettings.resolution.height)
            {
                currResIndex = i;
            }
        }

        //Updates all of the options components
        resDropdown.AddOptions(resolutionStrings);
        resDropdown.value = currResIndex;
        resDropdown.RefreshShownValue();

        volumeSlider.value = storedSettings.volume;
        fullscreenToggle.isOn = storedSettings.fullscreen;
        qualityDropdown.value = storedSettings.quality;
    }

    void Update()
    {
        if(input.ButtonDown("Cancel") && showingBindings)
        {
            //If the player presses cancel and the key-bindings are showing, the key-bindings are disabled and the settings are updated with the new bindings
            showingBindings = false;
            bindingMenu.SetActive(false);

            storedSettings.UpdateBindings();
        }

        if(saveMenu.activeSelf && input.ButtonDown("Cancel"))
        {
            //If the same menu is being shown and the player presses cancel, the save menu is disabled
            ShowSaves();
        }
    }

    private void FixedUpdate()
    {
        if (loadingOptions)
        {
            //If the menu is loading the options or going back to the main menu, the camera moves towards the target
            mainCamera.transform.position = new Vector3(Mathf.Lerp(mainCamera.transform.position.x, targetPos.x, loadSpeed), Mathf.Lerp(mainCamera.transform.position.y, targetPos.y, loadSpeed), normalPosition.z);

            //If the camera is close enough to the target, it stops moving
            if (Vector3.Distance(mainCamera.transform.position, targetPos) <= distError)
            {
                mainCamera.transform.position = targetPos;
                targetPos = Vector3.zero;
                loadingOptions = false;
            }
        }
    }

    //Scene management

    public void QuitGame()
    {
        //Quits the game
        Application.Quit();
    }

    public void NewGame()
    {
        //Starts the game at the first level
        SceneManager.LoadScene(firstLevel);
    }

    //Menu Management

    public void ShowSaves()
    {
        //Shows the save menu
        saveMenu.SetActive(!saveMenu.activeSelf);
    }

    public void LoadOptions()
    {
        //Begins to load the options menu, sets the target
        targetPos = optionsPosition;
        loadingOptions = true;
    }

    public void BackToMenu()
    {
        //Begins to move back to the main menu, sets the target as such and updates all of the settings
        targetPos = normalPosition;
        loadingOptions = true;

        storedSettings.UpdateBindings();
        SaveSettingsProcess.SaveSettings(storedSettings);
    }

    public void ShowBindings()
    {
        //Shows the bindings menu
        showingBindings = true;
        bindingMenu.SetActive(true);
    }

    //Options Management

    public void SetVolume(float volume)
    {
        //Sets the volume when the slider is adjusted
        audioMix.SetFloat("MasterVolume", volume);
        storedSettings.volume = volume;
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
        Resolution res = usableResolutions[resIndex];

        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
