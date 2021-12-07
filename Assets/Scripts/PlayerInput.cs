using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    private SolitaireManager _solitaireManager;
    private ScoreManager _scoreManager;
    private SpriteController _spriteController;
    private StockSpriteController _stockSpriteController;
    private GameObject _selectedCardGO;

    void Awake()
    {
        _solitaireManager = GetComponent<SolitaireManager>();
        _scoreManager = GetComponent<ScoreManager>();

        _selectedCardGO = this.gameObject;
    }

    void Start()
    {
        _spriteController = GameObject
            .FindGameObjectWithTag("Card")
            .GetComponent<SpriteController>();

        _stockSpriteController = GameObject
            .FindGameObjectWithTag("Stock")
            .GetComponent<StockSpriteController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GetMouseClick();
    }

    private void GetMouseClick()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition), 
            Vector2.zero);

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (hit)
            {
                if (hit.collider.CompareTag("Stock"))
                    OnStockClick();
                else if (hit.collider.CompareTag("Foundation"))
                    OnFoundationClick(hit.collider.gameObject);
                else if (hit.collider.CompareTag("Tableau"))
                    OnTableauClick(hit.collider.gameObject);
                else if (hit.collider.CompareTag("Card"))
                    OnCardClick(hit.collider.gameObject); 
            }
        }
    }
    
    private void OnStockClick()
    {
        _solitaireManager.DrawTalon();
        _stockSpriteController.ToggleSprite();

        // In case the player selects a card from the Talon
        // and then clicks on the Stock
        if (_selectedCardGO != this.gameObject) 
        {
            _spriteController.ToggleHighlight(_selectedCardGO);
            _selectedCardGO = this.gameObject;
        }
    }

    private void OnFoundationClick(GameObject clickedPile)
    {
        if (IsValidSequence(clickedPile))
            BuildSequence(clickedPile);
    }

    private void OnTableauClick(GameObject clickedPile)
    {
        if (IsValidSequence(clickedPile))
            BuildSequence(clickedPile);
    }

    private void OnCardClick(GameObject clickedCard)
    {
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();

        if (!cardPeek.IsFaceUp && IsTopCard(clickedCard))
        {
            cardPeek.SetFaceUp();

            // Prevent player from automatically selecting the flipped card
            return;
        }

        if (_selectedCardGO == this.gameObject)
        {
            if (cardPeek.IsFaceUp)
            {
                if (cardPeek.IsInTalonPile)
                {
                    // If clickedCard is from the Talon, only the top card can be selectable
                    if (IsTopCard(clickedCard))
                    {
                        _spriteController.ToggleHighlight(clickedCard);
                        _selectedCardGO = clickedCard;
                    }
                }
                else 
                {
                    _spriteController.ToggleHighlight(clickedCard);
                    _selectedCardGO = clickedCard;
                }
            }
        }
        // If clickedCard and the previously selected card are the same,
        // then deselect that card
        else if (clickedCard == _selectedCardGO)
        {
            _spriteController.ToggleHighlight(clickedCard);
            _selectedCardGO = this.gameObject;
        }
        // Otherwise, both cards are different,
        // so check if the previously selected card can move to clickedCard's pile
        else if (clickedCard != _selectedCardGO)
        {
            if (IsValidSequence(clickedCard))
                BuildSequence(clickedCard);
        }
    }

    private bool IsTopCard(GameObject clickedCard)
    {
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();

        // Returns true if clickedCard is the top card from either a Tableau pile or Talon
        // (to prevent players from interacting with the other cards in the pile)
        if (cardPeek.IsInTalonPile)
        {
            string talonCard = _solitaireManager.GetLastCard(-1);

            if (talonCard != null)
            {
                if (cardPeek.name == talonCard)
                    return true;
            }
            else
                Debug.LogError("_solitaire.talonPileList returned null");
        }
        else if (!cardPeek.IsFaceUp)
        {
            string tableauCard = _solitaireManager.GetLastCard(cardPeek.PileIndex);

            if (tableauCard != null)
            {
                if (cardPeek.name == tableauCard)
                    return true;
            }
            else
                Debug.LogError("_solitaire.tableauPilesList returned null");
        }

        return false;
    }

    private bool IsValidSequence(GameObject clickedCard)
    {
        // Ignore method if player is deselecting
        if (_selectedCardGO == this.gameObject)
            return false;

        CardItem cardToMove = _selectedCardGO.GetComponent<CardItem>();
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();

        if (cardPeek.IsInTalonPile)
            return false;

        bool cardMoveIsBlack =
            (cardToMove.Suit == "C" || cardToMove.Suit == "S");
        bool cardPeekIsBlack =
            (cardPeek.Suit == "C" || cardPeek.Suit == "S");

        if (cardPeek.IsInFoundationPile)
        {
            // Moving to an empty Foundation: Card must be an Ace
            if (cardPeek.Value == 0 && cardToMove.Value == 1)
                return true;
            // Moving to: Both cards must belong to the same suit
            // and be in consecutive ascending order
            else if ((cardPeek.Suit == cardToMove.Suit)
                && (cardToMove.Value == (cardPeek.Value + 1)))                 
                    return true;
        }
        // Moving to an empty Tableau pile: Card must be a King
        else if (cardPeek.Value == 14 && cardToMove.Value == 13)
            return true;
        // Moving to a non-empty Tableau pile: Both cards must be opposite colors
        // and be in consecutive descending order
        else if ((cardPeekIsBlack ^ cardMoveIsBlack)
            && (cardToMove.Value == (cardPeek.Value - 1))) 
                return true;

        Debug.Log("Invalid sequence: Card " + cardToMove.Suit + cardToMove.Value 
            + " to Card " + cardPeek.Suit + cardPeek.Value);

        return false;
    }

    private void BuildSequence(GameObject clickedCard)
    {
        CardItem cardToMove = _selectedCardGO.GetComponent<CardItem>();
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();
        float yOffset = 0.3f;
        float zOffset = 0.02f;

        // Set yOffset to 0 for empty and Foundation piles
        if (cardPeek.IsInFoundationPile
            || (!cardPeek.IsInFoundationPile && cardToMove.Value == 13))
                yOffset = 0;

        cardToMove.transform.position = new Vector3(
            clickedCard.transform.position.x, 
            clickedCard.transform.position.y - yOffset, 
            clickedCard.transform.position.z - zOffset);
        
        // Set selectedCard as the Child of clickedCard
        // (makes it easier to move sequences as they grow)
        cardToMove.transform.parent = clickedCard.transform;

        // Leaving Talon
        if (cardToMove.IsInTalonPile)
        {
            if (_solitaireManager.RemoveCard(cardToMove.name, -1))
                cardToMove.HasLeftTalonPile();
            else
                Debug.LogError("Failed to remove Card " + cardToMove.name + " from Talon List");
        }
        // Leaving Foundation
        else if (cardToMove.IsInFoundationPile && !cardPeek.IsInFoundationPile)
        {
            cardToMove.UpdateFoundationStatus();

            _scoreManager.TallyFoundation(cardToMove.Value * -1);
        }

        // Moving to Foundation
        if (cardPeek.IsInFoundationPile) 
        {
            cardToMove.UpdateFoundationStatus();

            _scoreManager.TallyFoundation(cardToMove.Value);
        }

        // Leaving a Tableau pile: Remove from list if it was part of the initial pile
        if (_solitaireManager.RemoveCard(cardToMove.name, cardToMove.PileIndex))
        {
            Debug.Log("Removing Card " + cardToMove.name 
                + " from Tableau List " + cardToMove.PileIndex);
        }

        // Move is done; update pilePos and reset selectedCard
        cardToMove.PileIndex = cardPeek.PileIndex;

        _spriteController.ToggleHighlight(_selectedCardGO);
        _selectedCardGO = this.gameObject;
    }
}
