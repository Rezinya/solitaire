using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolitaireManager : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject stockPileGO;
    public GameObject[] foundationPilesGO;
    public GameObject[] tableauPilesGO;
    

    public List<string>[] tableauPilesList;
    public List<string> talonPileList = new List<string>();
    public bool isStockEmpty = false;
    
    private List<string> _deck;
    private static string[] _suits = new string[] { "C", "D", "H", "S" };
    private static string[] _rankings = new string[] { "A", "2", "3", "4", 
        "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    
    private int _stockPileIndex;
    private int _drawThree;
    private int _drawRemainder;
    private List<string> _discardPileList = new List<string>();
    private List<List<string>> _stockSetsList = new List<List<string>>();
    private List<string> _tableauList0 = new List<string>();
    private List<string> _tableauList1 = new List<string>();
    private List<string> _tableauList2 = new List<string>();
    private List<string> _tableauList3 = new List<string>();
    private List<string> _tableauList4 = new List<string>();
    private List<string> _tableauList5 = new List<string>();
    private List<string> _tableauList6 = new List<string>();

    void Start()
    {
        // At the beginning of each game, the Tableau must be dealt
        tableauPilesList = new List<string>[] { _tableauList0, 
            _tableauList1, _tableauList2, _tableauList3, _tableauList4, 
            _tableauList5, _tableauList6 };

        // We create a standard 52-card deck, shuffle it...
        _deck = GenerateDeck();
        Shuffle(_deck);

        // ...Then form the seven bottom piles
        SetUpTableau();
        StartCoroutine(DealTableau());

        SortStockIntoSetsOfThree();
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();

        foreach (string s in _suits) 
        {
            foreach (string r in _rankings)
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
        // Fill out Tableau array with shuffled deck
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                tableauPilesList[j].Add(_deck.Last<string>());
                _deck.RemoveAt(_deck.Count - 1);
            }
        }
    }

    IEnumerator DealTableau()
    {
        // Deal cards to their respective Tableau pile
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float zOffset = 0.02f;

            foreach (string card in tableauPilesList[i])
            {
                yield return new WaitForSeconds(0.01f);

                GameObject newCard = Instantiate(
                    cardPrefab, 
                    new Vector3(tableauPilesGO[i].transform.position.x, 
                        tableauPilesGO[i].transform.position.y - yOffset, 
                        tableauPilesGO[i].transform.position.z - zOffset), 
                    Quaternion.identity, 
                    tableauPilesGO[i].transform);

                newCard.name = card;
                newCard.GetComponent<CardItem>().pileIndex = i;

                // Set the last card of each pile to be face-up
                if (card == tableauPilesList[i][tableauPilesList[i].Count - 1])
                    newCard.GetComponent<CardItem>().isFaceUp = true;

                _discardPileList.Add(card);

                yOffset += 0.2f;
                zOffset += 0.02f;
            }
        }

        // Any cards placed in the Tableau are then removed from the deck
        foreach (string card in _discardPileList)
        {
            if (_deck.Contains(card))
                _deck.Remove(card);
        }

        _discardPileList.Clear();
    }

    public void SortStockIntoSetsOfThree()
    {
        // If the player chooses to draw by three,
        // we split the Stock (that is, the remaining cards in deck)
        // into sets of up to three cards each
        _drawThree = _deck.Count / 3;
        _drawRemainder = _deck.Count % 3;
        _stockSetsList.Clear();

        int modifier = 0;

        for (int i = 0; i < _drawThree; i++)
        {
            List<string> newSet = new List<string>();

            for (int j = 0; j < 3; j++)
            {
                newSet.Add(_deck[j + modifier]);
            }

            // Add set to stockSets
            _stockSetsList.Add(newSet);
            modifier += 3;
        }

        // Get last set of Stock if there is a remainder
        if (_drawRemainder != 0)
        {
            List<string> setRemainder = new List<string>();
            modifier = 0;

            for (int i = 0; i < _drawRemainder; i++)
            {
                setRemainder.Add(_deck[_deck.Count 
                    - _drawRemainder 
                    + modifier]);
                modifier++;
            }

            // And add this last set to stockSets
            _stockSetsList.Add(setRemainder);

            // drawThree will be used to keep track of how many times 
            // the player can draw from the Stock
            _drawThree++;
        }

        _stockPileIndex = 0;
    }

    public void DealTalonByThree()
    {
        foreach (Transform child in stockPileGO.transform)
        {
            // Drawn cards are removed from the deck
            // and added to to the discardPile
            _deck.Remove(child.name);

            // We will use the discardPile to track which cards
            // are still playable and then sent back to Stock
            _discardPileList.Add(child.name);

            // Delete instantiated cards
            Destroy(child.gameObject);
        }

        // Form the Talon pile by drawing a set of three cards
        if (_stockPileIndex < _drawThree)
        {
            isStockEmpty = false;
            talonPileList.Clear();

            float xOffset = 2.5f;
            float zOffset = 0.02f;

            foreach (string card in _stockSetsList[_stockPileIndex])
            {
                GameObject newCard = Instantiate(
                    cardPrefab, 
                    new Vector3(stockPileGO.transform.position.x + xOffset, 
                        stockPileGO.transform.position.y, 
                        stockPileGO.transform.position.z - zOffset), 
                    Quaternion.identity, 
                    stockPileGO.transform);

                newCard.name = card;
                newCard.GetComponent<CardItem>().isFaceUp = true;
                newCard.GetComponent<CardItem>().isInTalonPile = true;

                talonPileList.Add(card);

                xOffset += 0.5f;
                zOffset += 0.02f;
            }

            _stockPileIndex++;
        }
        else
        {
            // Debug.Log("Stock is now empty.");
            isStockEmpty = true;

            ResetStock();
        }
    }

    public void ResetStock()
    {
        // We add the discardPile back into the deck
        // to be played (and sorted) again
        foreach (string card in _discardPileList)
        {
            _deck.Add(card);
        }

        _discardPileList.Clear();
        SortStockIntoSetsOfThree();
    }
}
