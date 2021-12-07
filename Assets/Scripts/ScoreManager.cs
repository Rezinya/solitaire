using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _totalValue;
    
    public GameObject WinPanel;
    public GameObject NewGameButton;

    void Start()
    {
        _totalValue = 0;
        
        WinPanel.SetActive(false);
    }

    public void TallyFoundation(int value) 
    {
        _totalValue += value;

        Debug.Log("Foundation value = " + _totalValue);

        if (_totalValue == 52) 
        {
            WinPanel.SetActive(true);
            NewGameButton.SetActive(false);
        }
    }
}
