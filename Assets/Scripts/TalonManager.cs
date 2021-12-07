using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalonManager : MonoBehaviour
{
    private int _drawThree;
    private int _stockPileIndex;

    private SolitaireManager _solitaireManager;

    public DrawNumberToggle DrawNumberToggle;

    private void Start()
    {
        _solitaireManager = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent<SolitaireManager>();
    }
    public void DrawTalon()
    {
        // TODO : Consolidate both methods
        if (DrawNumberToggle.IsDrawingByOne)
        {
            _stockPileIndex = 0;

            DrawTalonByOne();
        }
        else
            DrawTalonByThree();
    }

    private void DrawTalonByOne() { }

    private void DrawTalonByThree() { }
}
