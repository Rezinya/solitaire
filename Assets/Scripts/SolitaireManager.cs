using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolitaireManager : MonoBehaviour
{
    private List<string> _deck;
    private static string[] s_suits = new string[] { "C", "D", "H", "S" };
    private static string[] s_rankings = 
        new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    private List<List<string>> _stockSetsList = new List<List<string>>();
    private List<string> _talonPileList = new List<string>();
    private List<string> _discardPileList = new List<string>();
    
    private List<string>[] _tableauPilesList;
    private List<string> _tableauList0 = new List<string>();
    private List<string> _tableauList1 = new List<string>();
    private List<string> _tableauList2 = new List<string>();
    private List<string> _tableauList3 = new List<string>();
    private List<string> _tableauList4 = new List<string>();
    private List<string> _tableauList5 = new List<string>();
    private List<string> _tableauList6 = new List<string>();
    
    public DrawNumberToggle DrawNumberToggle;
    private int _drawThree;
    private int _stockPileIndex;
    
    public Sprite[] CardFaces;
    public GameObject CardPrefab;
    public GameObject StockPileGO;
    public GameObject[] FoundationPilesGO;
    public GameObject[] TableauPilesGO;

    public bool IsStockEmpty { get; private set; }

    void Awake()
    {
        _tableauPilesList = new List<string>[] { _tableauList0, _tableauList1, _tableauList2, 
            _tableauList3, _tableauList4, _tableauList5, _tableauList6 };
    }

    void Start()
    {
        _deck = GenerateDeck();
        Shuffle(_deck);

        SetUpTableau();
        StartCoroutine(DealTableau());

        if (!DrawNumberToggle.IsDrawingByOne)
            SortStockIntoSetsOfThree();
    }

    public static List<string> GenerateDeck()
    {
        List<string> new_deck = new List<string>();

        foreach (string s in s_suits) 
        {
            foreach (string r in s_rankings)
            {
                new_deck.Add(s + r);
            }
        }

        return new_deck;
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
                _tableauPilesList[j].Add(_deck.Last<string>());
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

            foreach (string card in _tableauPilesList[i])
            {
                yield return new WaitForSeconds(0.01f);

                GameObject newCard = Instantiate(
                    CardPrefab, 
                    new Vector3(TableauPilesGO[i].transform.position.x, 
                        TableauPilesGO[i].transform.position.y - yOffset, 
                        TableauPilesGO[i].transform.position.z - zOffset), 
                    Quaternion.identity, 
                    TableauPilesGO[i].transform);

                newCard.name = card;
                newCard.GetComponent<CardItem>().PileIndex = i;

                // Set the last card of each pile to be face-up
                if (card == _tableauPilesList[i][_tableauPilesList[i].Count - 1])
                    newCard.GetComponent<CardItem>().SetFaceUp();

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
    
    public void DrawTalon()
    {
        if (DrawNumberToggle.IsDrawingByOne)
        {
            _stockPileIndex = 0;

            DrawTalonByOne();
        }
        else 
            DrawTalonByThree();
    }

    public bool RemoveCard(string name, int pileIndex) 
    {
        if (pileIndex < 0)
            return _talonPileList.Remove(name);
        else
            return _tableauPilesList[pileIndex].Remove(name);
    }

    public string GetLastCard(int pileIndex)
    {
        string card = null;

        if (pileIndex < 0)
            card = _talonPileList.LastOrDefault();
        else
            card = _tableauPilesList[pileIndex].LastOrDefault();

        return card;
    }

    private void DrawTalonByOne() 
    {
        foreach (Transform child in StockPileGO.transform)
        {
            // Drawn cards are removed from the _deck and added to to the discardPile
            _deck.Remove(child.name);

            // We will use the discardPile to track which cards are still playable
            // and then sent back to Stock
            _discardPileList.Add(child.name);

            // Delete instantiated cards
            Destroy(child.gameObject);
        }

        // Form the Talon
        if (_stockPileIndex < _deck.Count)
        {
            IsStockEmpty = false;
            _talonPileList.Clear();

            float xOffset = 2.5f;
            float zOffset = 0.02f;

            foreach (string card in _deck)
            {
                GameObject newCard = Instantiate(
                    CardPrefab,
                    new Vector3(StockPileGO.transform.position.x + xOffset,
                        StockPileGO.transform.position.y,
                        StockPileGO.transform.position.z - zOffset),
                    Quaternion.identity,
                    StockPileGO.transform);

                newCard.name = card;
                newCard.GetComponent<CardItem>().InstantiatedInTalon();

                _talonPileList.Add(card);
            }

            _stockPileIndex++;
        }
        else
        {
            IsStockEmpty = true;

            ResetStock();
        }
    }

    private void SortStockIntoSetsOfThree()
    {
        // If the player chooses to draw by three, we split the Stock
        // (that is, the remaining cards in _deck) into sets of up to three cards each
        int drawRemainder = _deck.Count % 3;
        int modifier = 0;

        _drawThree = _deck.Count / 3;
        _stockSetsList.Clear();

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
        if (drawRemainder != 0)
        {
            List<string> setRemainder = new List<string>();
            modifier = 0;

            for (int i = 0; i < drawRemainder; i++)
            {
                setRemainder.Add(_deck[_deck.Count - drawRemainder + modifier]);
                modifier++;
            }

            // And add this last set to stockSets
            _stockSetsList.Add(setRemainder);
        }

        _stockPileIndex = 0;
    }

    private void DrawTalonByThree()
    {
        foreach (Transform child in StockPileGO.transform)
        {
            // Drawn cards are removed from the _deck and added to to the discardPile
            _deck.Remove(child.name);

            // We will use the discardPile to track which cards are still playable
            // and then sent back to Stock
            _discardPileList.Add(child.name);

            // Delete instantiated cards
            Destroy(child.gameObject);
        }

        // Form the Talon by drawing a set of three cards
        if (_stockPileIndex < _drawThree)
        {
            IsStockEmpty = false;
            _talonPileList.Clear();

            float xOffset = 2.5f;
            float zOffset = 0.02f;

            foreach (string card in _stockSetsList[_stockPileIndex])
            {
                GameObject newCard = Instantiate(
                    CardPrefab, 
                    new Vector3(StockPileGO.transform.position.x + xOffset, 
                        StockPileGO.transform.position.y, 
                        StockPileGO.transform.position.z - zOffset), 
                    Quaternion.identity, 
                    StockPileGO.transform);

                newCard.name = card;
                newCard.GetComponent<CardItem>().InstantiatedInTalon();

                _talonPileList.Add(card);

                xOffset += 0.5f;
                zOffset += 0.02f;
            }

            _stockPileIndex++;
        }
        else
        {
            IsStockEmpty = true;

            ResetStock();
        }
    }

    private void ResetStock()
    {
        _deck.Clear();

        foreach (string card in _discardPileList)
        {
            _deck.Add(card);
        }

        _discardPileList.Clear();

        if (!DrawNumberToggle.IsDrawingByOne)
            SortStockIntoSetsOfThree();
    }
}
