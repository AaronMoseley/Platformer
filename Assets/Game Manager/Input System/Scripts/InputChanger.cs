using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InputChanger : MonoBehaviour
{
    //Allows the player to change the binding of any control

    [Header("UI Settings")]
    public Text buttonText;
    public Text buttonTitle;
    bool selected;
    [Space]

    [Header("Button Information")]
    public string buttonName;
    int buttonNum;
    [Space]

    GameObject[] otherChangers;
    KeyCode[] mouseKeys;

    InputManager inputManager;

    void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();

        //Finds any other input changes and makes sure to log any mouse key inputs for later
        otherChangers = GameObject.FindGameObjectsWithTag("Input Changer");
        mouseKeys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6 };

        //Finds the button this script corresponds to
        for (int i = 0; i < inputManager.buttons.Count; i++)
        {
            if(inputManager.buttons[i].name == buttonName)
            {
                buttonNum = i;
            }
        }

        //Sets the requisite UI elements
        buttonText.text = inputManager.buttons[buttonNum].code.ToString();
        buttonTitle.text = inputManager.buttons[buttonNum].name;
    }

    void OnGUI()
    {
        //Triggers after the script is selecta and when the player clicks a new button
        Event e = Event.current;

        //Finds the button or mouse key the player pressed and then changes the corresponding input button to have that keycode
        if ((e.isKey || e.isMouse) && selected)
        {
            if (e.isMouse)
            {
                if (e.type == EventType.MouseDown)
                {
                    for (int i = 0; i < mouseKeys.Length; ++i)
                    {
                        if (Input.GetKeyDown(mouseKeys[i]))
                        {
                            ChangeCode(mouseKeys[i]);
                            break;
                        }
                    }
                }
            }
            else
            {
                ChangeCode(e.keyCode);
            }

            buttonText.text = inputManager.buttons[buttonNum].code.ToString();
        }
    }

    public void Select()
    {
        //Activates when this button is clicked in the UI, selects this script and deselects all others
        selected = !selected;

        for(int i = 0; i < otherChangers.Length; i++)
        {
            if(otherChangers[i] != gameObject && otherChangers[i].GetComponent<InputChanger>().selected)
            {
                otherChangers[i].GetComponent<InputChanger>().SetSelected(false);
            }
        }
    }

    public void ChangeCode(KeyCode code)
    {
        //Changes the keycode of the button and deselects this script
        inputManager.buttons[buttonNum].code = code;
        selected = false;
    }

    public void SetSelected(bool state)
    {
        selected = state;
    }
}
