using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private SolitaireManager _solitaireManager;
    private CardItem _cardItem;
    private Sprite _cardFace;
    private SpriteRenderer _spriteRenderer;
    
    public Sprite cardBack;

    void Awake()
    {
        _cardItem = GetComponent<CardItem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _solitaireManager = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent<SolitaireManager>();

        List<string> deck = SolitaireManager.GenerateDeck();

        int i = 0;

        foreach (string card in deck) 
        {
            if (this.name == card) 
            {
                _cardFace = _solitaireManager.CardFaces[i];
                break;
            }

            i++;
        }
        
        ToggleSprite();
    }

    public void ToggleSprite() 
    {
        if (_cardItem.IsFaceUp)
            _spriteRenderer.sprite = _cardFace;
        else
            _spriteRenderer.sprite = cardBack;
    }

    public void ToggleHighlight(GameObject clickedObject)
    {
        SpriteRenderer clickedSR = clickedObject.GetComponent<SpriteRenderer>();

        if (clickedSR.color != Color.yellow)
            clickedSR.color = Color.yellow;
        else
            clickedSR.color = Color.white;
    }
}
