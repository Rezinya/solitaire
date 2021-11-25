using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardItem : MonoBehaviour
{
    public string suit;
    public int value;
    public int pileIndex;

    public bool isFaceUp = false;
    public bool isInTalonPile = false;
    public bool isInFoundationPile = false;

    void Start()
    {
        if (CompareTag("Card"))
        {
            suit = transform.name[0].ToString();

            char c = transform.name[1];

            if (c == 'A')
                value = 1;
            else if (c == '1')
                value = 10;
            else if (c == 'J')
                value = 11;
            else if (c == 'Q')
                value = 12;
            else if (c == 'K')
                value = 13;
            else
                value = (int)char.GetNumericValue(c); 
        }
    }
}
