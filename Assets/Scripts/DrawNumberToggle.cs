using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawNumberToggle : MonoBehaviour
{
    private int _drawNum;
    private Toggle _drawOneToggle;
    private Toggle _drawThreeToggle;
    
    public bool IsDrawingByOne;

    void Start()
    {
        if (CompareTag("DrawOne"))
        {
            _drawOneToggle = GetComponent<Toggle>();

            _drawOneToggle
                .onValueChanged
                .AddListener(delegate { SetDrawNumberOne(); });
        }
        else if (CompareTag("DrawThree")) 
        {
            _drawThreeToggle = GetComponent<Toggle>();

            _drawThreeToggle
                .onValueChanged
                .AddListener(delegate { SetDrawNumberThree(); });
        }

        // Default draw number is set to three
        _drawNum = PlayerPrefs.GetInt("DrawNumber", 3);

        if (_drawNum == 3)
            IsDrawingByOne = false;
        else
            IsDrawingByOne = true;
    }

    public void SetDrawNumberOne()
    {
        if (_drawOneToggle.isOn)
        {
            // Debug.Log("Number of cards drawn is now set to: One");

            PlayerPrefs.SetInt("DrawNumber", 1);
            PlayerPrefs.Save();
        }
    }

    public void SetDrawNumberThree()
    { 
        if (_drawThreeToggle.isOn) 
        {
            // Debug.Log("Number of cards drawn is now set to: Three");

            PlayerPrefs.SetInt("DrawNumber", 3);
            PlayerPrefs.Save();
        }
    }
}
