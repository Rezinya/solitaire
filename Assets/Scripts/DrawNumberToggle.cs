using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawNumberToggle : MonoBehaviour
{
    private int _drawNum;
    private Toggle _drawOneToggle;
    private Toggle _drawThreeToggle;
    
    public bool IsDrawingByThree;

    void Start()
    {
        Component[] toggles = GetComponentsInChildren(typeof(Toggle), true);

        foreach (Toggle toggle in toggles) 
        {
            if (toggle.gameObject.CompareTag("DrawOne"))
            {
                _drawOneToggle = toggle;
                _drawOneToggle.onValueChanged.AddListener((isOn) => ToggleGroupValueChanged(1));
            }
            else if (toggle.gameObject.CompareTag("DrawThree")) 
            { 
                _drawThreeToggle = toggle;
                _drawThreeToggle.onValueChanged.AddListener((isOn) => ToggleGroupValueChanged(3));
            }
        }

        // Get draw number -- Default is set to 3
        _drawNum = PlayerPrefs.GetInt("DrawNumber", 3);

        if (_drawNum == 1)
            IsDrawingByThree = false;
        else
            IsDrawingByThree = true;
    }

    private void ToggleGroupValueChanged(int number) 
    {
        if (number == 1) 
        {
            PlayerPrefs.SetInt("DrawNumber", 1);
            PlayerPrefs.Save();
        }
        else 
        {
            PlayerPrefs.SetInt("DrawNumber", 3);
            PlayerPrefs.Save();
        }
    }
}
