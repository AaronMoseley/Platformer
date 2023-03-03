using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Button", menuName = "Input", order = 1)]
public class InputButton : ScriptableObject
{
    public string buttonName;
    public KeyCode code;
    public string axis;
}
