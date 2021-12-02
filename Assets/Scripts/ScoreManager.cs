using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject newGameButton;

    private int totalValue;

    private void Awake()
    {
        totalValue = 0;
    }

    void Start()
    {
        winPanel.SetActive(false);
    }

    public void TallyFoundation(int value) 
    {
        totalValue += value;

        Debug.Log("Foundation value = " + totalValue);

        if (totalValue == 52) 
        {
            winPanel.SetActive(true);
            newGameButton.SetActive(false);
        }
    }
}
