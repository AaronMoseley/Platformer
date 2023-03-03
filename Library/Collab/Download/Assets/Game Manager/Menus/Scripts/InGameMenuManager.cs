using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public int mainMenuSceneNum;

    InputManager input;
    GameObject player;

    public GameObject bindingMenu;
    public GameObject optionsMenu;
    public GameObject saveMenu;
    public GameObject overrideMenu;
    public GameObject loadMenu;

    public AudioMixer audioMix;

    public Dropdown resDropdown;
    Resolution[] resolutions;

    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;

    public string showing = "none";

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
            SavedSettings data = SaveSettingsProcess.LoadSettings();

            storedSettings.resolution.height = data.resHeight;
            storedSettings.resolution.width = data.resWidth;
            storedSettings.fullscreen = data.fullscreen;
            storedSettings.quality = data.quality;
            storedSettings.volume = data.volume;
            storedSettings.keyBindings = data.bindings;

            for(int i = 0; i < input.buttons.Count; i++)
            {
                for (int j = 0; j < data.bindings.Length; j++)
                {
                    if (input.buttons[i].name == data.bindings[j][0])
                    {
                        input.buttons[i].code = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.bindings[i][1]);
                    }
                }

                if(i >= input.buttons.Count)
                {
                    int j = i - input.buttons.Count;

                    input.hotkeys[j].name = data.bindings[i][0];
                    input.hotkeys[j].code = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.bindings[i][1]);
                }
            }
        }

        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> resolutionStrings = new List<string>();

        int currResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionStrings.Add(option);

            if (resolutions[i].width == storedSettings.resolution.width && resolutions[i].height == storedSettings.resolution.height)
            {
                currResIndex = i;
            }
        }

        resDropdown.AddOptions(resolutionStrings);
        resDropdown.value = currResIndex;
        resDropdown.RefreshShownValue();

        volumeSlider.value = storedSettings.volume;
        fullscreenToggle.isOn = storedSettings.fullscreen;
        qualityDropdown.value = storedSettings.quality;
    }

    void Update()
    {
        if(input.ButtonDown("Cancel") && !player.GetComponentInChildren<Inventory>().invOpen && showing == "none" && !player.GetComponent<Movement>().talking)
        {
            player.GetComponent<Movement>().enabled = false;
            player.GetComponentInChildren<GunManager>().enabled = false;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            showing = "pause";
        } else if(input.ButtonDown("Cancel") && showing == "pause")
        {
            player.GetComponent<Movement>().enabled = true;
            player.GetComponentInChildren<GunManager>().enabled = true;
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            showing = "none";
        } else if(input.Button("Cancel") && showing == "loads")
        {
            loadMenu.SetActive(false);
            showing = "pause";
        }

        if(input.ButtonDown("Cancel") && showing == "options")
        {
            pauseMenu.SetActive(true);
            bindingMenu.SetActive(false);
            optionsMenu.SetActive(false);

            SaveSettingsProcess.SaveSettings(storedSettings);

            showing = "pause";
        } else if(input.ButtonDown("Cancel") && showing == "bindings")
        {
            bindingMenu.SetActive(false);
            optionsMenu.SetActive(false);
            optionsMenu.SetActive(true);

            storedSettings.UpdateBindings();

            showing = "options";
        }
    }

    public void PlayerDie()
    {
        pauseMenu.SetActive(false);
        deathMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneNum);
    }

    public void LoadOptions()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        bindingMenu.SetActive(false);
        showing = "options";
    }

    public void SetVolume(float volume)
    {
        storedSettings.volume = volume;
        audioMix.SetFloat("MasterVolume", volume);
    }

    public void SetQuality(int quality)
    {
        storedSettings.quality = quality;
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        storedSettings.fullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resIndex)
    {
        Resolution res = resolutions[resIndex];
        storedSettings.resolution = res;
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void ShowBindings()
    {
        bindingMenu.SetActive(true);
        pauseMenu.SetActive(false);
        showing = "bindings";
    }

    public void ShowSaves(bool state)
    {
        saveMenu.SetActive(state);

        if (state)
        {
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
            showing = "none";
            player.GetComponent<Movement>().enabled = true;
            player.GetComponentInChildren<GunManager>().enabled = true;
            Time.timeScale = 1;
        }
    }

    public void CloseOverrideMenu()
    {
        overrideMenu.SetActive(false);
        saveSystem.currOverriding = -1;
    }

    public void ShowLoad()
    {
        loadMenu.SetActive(true);
        showing = "loads";
    }
}
