using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Creates a custom input system that allows the player to decide what keys correspond to which button

    [Header("Input Information/Settings")]
    public List<InputButton> buttons;
    public float lerpFactor;
    [Space]

    float xAxis = 0f;
    int xRaw = 0;

    float yAxis = 0f;
    int yRaw = 0;

    int posX;
    int negX;

    int posY;
    int negY;

    void Start()
    {
        SetAxes();

        GameObject.FindGameObjectWithTag("Stored Settings").GetComponent<SettingsData>().UpdateBindings();
    }

    public void SetAxes()
    {
        //Finds the buttons that correspond to the axes and logs them
        for (int i = 0; i < buttons.Count; i++)
        {
            switch (buttons[i].axis)
            {
                case "PosX":
                    posX = i;
                    break;
                case "NegX":
                    negX = i;
                    break;
                case "PosY":
                    posY = i;
                    break;
                case "NegY":
                    negY = i;
                    break;
            }
        }
    }

    void Update()
    {
        if(Input.GetKey(buttons[posX].code) && xAxis < 1)
        {
            //If the player is trying to go right and the x axis variable has not reached 1, lerp it towards 1 and add to x raw until it reaches 1
            if (xRaw < 1)
            {
                xRaw++;
            }

            if (xAxis > 0.99f)
            {
                //Set x axis to 1 if it's close enough
                xAxis = 1;
            }
            else
            {
                xAxis = Mathf.Lerp(xAxis, 1, lerpFactor);
            }
        }

        if(Input.GetKey(buttons[negX].code) && xAxis > -1)
        {
            //If the player is trying to go left and the x axis variable has not reached -1, lerp it towards -1 and subtract from x raw until it reaches -1
            if (xRaw > -1)
            {
                xRaw--;
            }

            if (xAxis < -0.99f)
            {
                xAxis = -1;
            }
            else
            {
                //Set x axis to -1 if it's close enough
                xAxis = Mathf.Lerp(xAxis, -1, lerpFactor);
            }
        }

        //If the player is not moving on the x axis, set x raw to 0 and lerp x axis toward 0
        if(!Input.GetKey(buttons[posX].code) && !Input.GetKey(buttons[negX].code))
        {
            if (xRaw != 0)
            {
                xRaw = 0;
            }

            if(xAxis > -0.01f && xAxis < 0)
            {
                xAxis = 0;
            } else if(xAxis < 0.01 && xAxis > 0)
            {
                xAxis = 0;
            } else {
                xAxis = Mathf.Lerp(xAxis, 0, lerpFactor);
            }
        }

        //Repeat the x axis just for y axis
        if(Input.GetKey(buttons[posY].code) && yAxis < 1)
        {
            if (yRaw < 1)
            {
                yRaw++;
            }

            if (yAxis > 0.99f)
            {
                yAxis = 0;
            }
            else
            {
                yAxis = Mathf.Lerp(yAxis, 1, lerpFactor);
            }
        }

        if(Input.GetKey(buttons[negY].code) && yAxis > -1)
        {
            if (yRaw > -1)
            {
                yRaw--;
            }

            if (yAxis < -0.99f)
            {
                yAxis = -1;
            }
            else
            {
                yAxis = Mathf.Lerp(yAxis, -1, lerpFactor);
            }
        }

        if (!Input.GetKey(buttons[posY].code) && !Input.GetKey(buttons[negY].code))
        {
            if (yRaw != 0)
            {
                yRaw = 0;
            }

            if (yAxis > -0.01f && yAxis < 0)
            {
                yAxis = 0;
            }
            else if (yAxis < 0.01f && yAxis > 0)
            {
                yAxis = 0;
            }
            else
            {
                yAxis = Mathf.Lerp(yAxis, 0, lerpFactor);
            }
        }
    }

    public bool ButtonDown(string name)
    {
        //Returns true if the button corresponding to the given string was pressed down on this frame
        KeyCode code = KeyCode.None;
        
        //Finds the keycode being referenced in the buttons array
        for(int i = 0; i < buttons.Count; i++)
        {
            if(buttons[i].name == name)
            {
                code = buttons[i].code;
            }
        }

        if (Input.GetKeyDown(code))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ButtonUp(string name)
    {
        //Returns true if the referenced button stops being pressed on this frame
        KeyCode code = KeyCode.None;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].name == name)
            {
                code = buttons[i].code;
            }
        }

        if (Input.GetKeyUp(code))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Button(string name)
    {
        //Returns true if the referenced button is being pressed at all
        KeyCode code = KeyCode.None;

        for(int i = 0; i < buttons.Count; i++)
        {
            if(buttons[i].name == name)
            {
                code = buttons[i].code;
            }
        }

        return Input.GetKey(code);
    }

    public int AxisRaw(string dir)
    {
        //Returns -1, 0, or 1 based on the player's desired direction
        if(dir.Equals("Horizontal"))
        {
            return xRaw;
        } else if(dir.Equals("Vertical"))
        {
            return yRaw;
        }

        return 0;
    }

    public float Axis(string dir)
    {
        //Returns a decimal between -1 and 1 that gets closer to extremes as the player holds the direction buttons
        if(dir.Equals("Horizontal"))
        {
            return xAxis;
        } else if(dir.Equals("Vertical"))
        {
            return yAxis;
        }

        return 0f;
    }
}
