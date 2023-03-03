using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public List<InputButton> buttons;

    float xAxis = 0f;
    int xRaw = 0;

    float yAxis = 0f;
    int yRaw = 0;

    public float lerpFactor;
    public float doubleClickInterval;

    int posX;
    int negX;
    int posY;
    int negY;

    void Start()
    {
        SetAxes();
    }

    public void SetAxes()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].axis == "PosX")
            {
                posX = i;
            }
            else if (buttons[i].axis == "NegX")
            {
                negX = i;
            }
            else if (buttons[i].axis == "PosY")
            {
                posY = i;
            }
            else if (buttons[i].axis == "NegY")
            {
                negY = i;
            }
        }
    }

    void Update()
    {
        if(Input.GetKey(buttons[posX].code) && xAxis < 1)
        {
            if (xRaw < 1)
            {
                xRaw++;
            }


            if (xAxis > 0.99f)
            {
                xAxis = 1;
            }
            else
            {
                xAxis = Mathf.Lerp(xAxis, 1, lerpFactor);
            }
        }

        if(Input.GetKey(buttons[negX].code) && xAxis > -1)
        {
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
                xAxis = Mathf.Lerp(xAxis, -1, lerpFactor);
            }
        }

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
        KeyCode code = KeyCode.None;
        
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
        if(dir == "Horizontal")
        {
            return xRaw;
        } else if(dir == "Vertical")
        {
            return yRaw;
        }

        return 0;
    }

    public float Axis(string dir)
    {
        if(dir == "Horizontal")
        {
            return xAxis;
        } else if(dir == "Vertical")
        {
            return yAxis;
        }

        return 0f;
    }
}
