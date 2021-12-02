using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawNumberToggle : MonoBehaviour
{
    public bool isDrawingByOne;

    private int drawNum;
    private Toggle _drawOneToggle;
    private Toggle _drawThreeToggle;

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
        drawNum = PlayerPrefs.GetInt("DrawNumber", 3);

        if (drawNum == 3)
        {
            Debug.Log("Draw number is currently set to: Three");

            isDrawingByOne = false;
        }
        else 
        {
            Debug.Log("Draw number is currently set to: One");

            isDrawingByOne = true;
        }

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
