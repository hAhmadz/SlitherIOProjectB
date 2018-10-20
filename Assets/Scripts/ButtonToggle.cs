using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public bool ButtonOn = false;
    public Button MyButton;

    public void ButtonClick()
    {
        ButtonOn = !ButtonOn;
        if (ButtonOn)
        {
            MyButton.image.color = new Color(207f, 64f, 46f, 255f);
        }
        else
        {
            MyButton.image.color = Color.white;
        }
    }
}