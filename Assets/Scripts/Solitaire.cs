using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] rankings = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public List<string> deck;
    public List<string>[] tableau;
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject[] bottomPilePos;
    public GameObject[] topPilePos;

    private List<string> bottomPile0 = new List<string>();
    private List<string> bottomPile1 = new List<string>();
    private List<string> bottomPile2 = new List<string>();
    private List<string> bottomPile3 = new List<string>();
    private List<string> bottomPile4 = new List<string>();
    private List<string> bottomPile5 = new List<string>();
    private List<string> bottomPile6 = new List<string>();

    void Start()
    {
        tableau = new List<string>[] { bottomPile0, bottomPile1, bottomPile2, bottomPile3,
            bottomPile4, bottomPile5, bottomPile6 };

        deck = GenerateDeck();
        Shuffle(deck);

        // [Test] Printing cards...
        foreach (string card in deck)
        {
            Debug.Log(card);
        }

        SetUpTableau();
        StartCoroutine(DealTableau());
    }

    void Update()
    {
        
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();

        foreach (string s in suits) 
        {
            foreach (string r in rankings)
            {
                newDeck.Add(s + r);
            }
        }

        return newDeck;
    }

    private static void Shuffle<T>(List<T> list)
    {
        // Shuffle the cards using the Fisher-Yates shuffle algorithm
        System.Random rng = new System.Random();
        int n = list.Count;

        while (n > 1)
        {
            n--;

            int k = rng.Next(n + 1);

            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    private void SetUpTableau()
    {
        /*  The tableau consists of seven piles. Starting from left to right, we place 
         *  the first card face-up to make the first pile, then deal one card face-down 
         *  for the next six piles. Starting again from left to right, we deal one card 
         *  face-up on the second pile, then one card face-down for the remaining five 
         *  piles. Reapeat this pattern until we have one card facing up for each pile.
         *  
         *  Piles that have more than one card are fanned downwards, allowing the player 
         *  to see how many cards are currently in that pile.
         **/

        // Fill out tableau array
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                tableau[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    IEnumerator DealTableau()
    {
        // Deal cards to respective pile
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float zOffset = 0.02f;

            foreach (string card in tableau[i])
            {
                yield return new WaitForSeconds(0.01f);

                GameObject newCard = Instantiate(cardPrefab,
                    new Vector3(bottomPilePos[i].transform.position.x, bottomPilePos[i].transform.position.y - yOffset, bottomPilePos[i].transform.position.z - zOffset),
                    Quaternion.identity, bottomPilePos[i].transform);
                newCard.name = card;

                // Set the last card to be face-up
                if (card == tableau[i][tableau[i].Count - 1])
                {
                    newCard.GetComponent<Playable>().isFaceUp = true;
                }
                
                yOffset += 0.2f;
                zOffset += 0.02f;
            }
        } 
    }
}
