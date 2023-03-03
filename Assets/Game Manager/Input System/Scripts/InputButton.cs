using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Button", menuName = "Input", order = 1)]
public class InputButton : ScriptableObject
{
    //Creates a new input button for the custom input system

    public string buttonName;
    public KeyCode code;
    public string axis = "none";
}
