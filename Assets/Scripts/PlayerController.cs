using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SolitaireManager _solitaireManager;
    private SpriteController _spriteController;
    private StockSpriteController _stockSpriteController;

    private GameObject _selectedCardGO;

    void Start()
    {
        _solitaireManager = FindObjectOfType<SolitaireManager>();
        _spriteController = FindObjectOfType<SpriteController>();
        _stockSpriteController = FindObjectOfType<StockSpriteController>();
        

        // Initialize selectedCard to SolitaireGame
        _selectedCardGO = this.gameObject;
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
    
    private void OnStockClick()
    {
        Debug.Log("Clicked on Stock");

        _solitaireManager.DealTalonByThree();
        _stockSpriteController.ToggleSprite();

        // Reset selectedCard in case the player selects a card
        // from the Talon and then clicks on the Stock
        if (_selectedCardGO != this.gameObject) 
        {
            Debug.Log("Deselecting Card: " + _selectedCardGO.name);

            _spriteController.ToggleHighlight(_selectedCardGO);
            _selectedCardGO = this.gameObject;
        }
    }

    private void OnFoundationClick(GameObject clickedPile)
    {
        Debug.Log("Clicked on empty Foundation pile");

        if (IsValidSequence(clickedPile))
            BuildSequence(clickedPile);
    }

    private void OnTableauClick(GameObject clickedPile)
    {
        Debug.Log("Clicked on empty Tableau pile");

        if (IsValidSequence(clickedPile))
            BuildSequence(clickedPile);
    }

    private void OnCardClick(GameObject clickedCard)
    {
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();

        // If a card is facing down, only the top card can be flipped over
        if (!cardPeek.isFaceUp && IsTopCard(clickedCard))
        {
            Debug.Log("Revealing Card " + cardPeek.name);

            cardPeek.isFaceUp = true;

            // Prevent player from automatically selecting this card
            return;
        }

        // No cards were previously selected...
        if (_selectedCardGO == this.gameObject)
        {
            // All selectable cards must be face up
            if (cardPeek.isFaceUp)
            {
                if (cardPeek.isInTalonPile)
                {
                    // If clickedCard is from the Talon,
                    // only the top card can be selectable
                    if (IsTopCard(clickedCard))
                    {
                        Debug.Log("Selecting card: " + clickedCard.name);

                        _spriteController.ToggleHighlight(clickedCard);
                        _selectedCardGO = clickedCard;
                    }
                }
                else 
                {
                    Debug.Log("Selecting card: " + clickedCard.name);

                    _spriteController.ToggleHighlight(clickedCard);
                    _selectedCardGO = clickedCard;
                }
            }
        }
        // If clickedCard and the previously selected card are the same...
        else if (clickedCard == _selectedCardGO)
        {
            // Then deselect that card
            Debug.Log("Deselecting card: " + clickedCard.name);

            _spriteController.ToggleHighlight(clickedCard);
            _selectedCardGO = this.gameObject;
        }
        // Otherwise, both cards are different...
        else if (clickedCard != _selectedCardGO)
        {
            // Check if the previously selected card
            // can move to clickedCard's pile
            if (IsValidSequence(clickedCard))
                BuildSequence(clickedCard);
        }
    }

    private bool IsTopCard(GameObject clickedCard)
    {
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();

        // Returns true if clickedCard is the top card from either
        // a Tableau pile or Talon (to prevent players from interacting
        // with the other cards in the pile)
        if (cardPeek.isInTalonPile)
        {
            string talonCard = _solitaireManager.talonPileList.LastOrDefault();

            if (talonCard != null)
            {
                if (cardPeek.name == talonCard)
                    return true;
                else
                {
                    Debug.Log("Unable to select Card " + cardPeek.name
                        + ": Blocked by " + talonCard);

                    return false;
                }
            }
            else
                Debug.LogError("_solitaire.talonPileList returned null");
        }
        else if (!cardPeek.isFaceUp)
        {
            string tableauCard = _solitaireManager
                .tableauPilesList[cardPeek.pileIndex]
                .LastOrDefault();

            if (tableauCard != null)
            {
                if (cardPeek.name == tableauCard)
                    return true;
                else
                {
                    Debug.Log("Unable to select Card " + cardPeek.name
                        + ": Blocked by " + tableauCard);

                    return false;
                }
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

        if (cardPeek.isInTalonPile)
        {
            Debug.Log("Can't build sequences on Talon.");

            return false;
        }
        
        string cardSuit = "";

        if (cardToMove.suit == "C")
            cardSuit = "Clubs";
        else if (cardToMove.suit == "D")
            cardSuit = "Diamonds";
        else if (cardToMove.suit == "H")
            cardSuit = "Hearts";
        else if (cardToMove.suit == "S")
            cardSuit = "Spades";

        bool cardMoveIsBlack =
            (cardToMove.suit == "C" || cardToMove.suit == "S");
        bool cardPeekIsBlack =
            (cardPeek.suit == "C" || cardPeek.suit == "S");

        // Moving to a Foundation pile...
        if (cardPeek.isInFoundationPile)
        {
            // Foundation is empty: Card must be an Ace
            if (cardPeek.value == 0 && cardToMove.value == 1)
            {
                Debug.Log("1) Valid: Moving Ace of " + cardSuit
                    + " to Foundation Pile " + cardPeek.pileIndex);

                return true;
            }
            // Foundation is not empty: Both cards must belong to the
            // same suit and be in consecutive ascending order
            else if ((cardPeek.suit == cardToMove.suit)
                && (cardToMove.value == (cardPeek.value + 1)))
            {
                Debug.Log("1) Valid: Moving Card "
                    + cardToMove.name + " to " + cardPeek.name);

                return true;
            }
        }
        // Moving to an empty Tableau pile: Card must be a King
        else if (cardPeek.value == 14 && cardToMove.value == 13)
        {
            Debug.Log("1) Valid: Moving King of " + cardSuit
                + " to Tableau Pile " + cardPeek.pileIndex);

            return true;
        }
        // Moving to a non-empty Tableau pile: Both cards must be
        // opposite colors and be in consecutive descending order
        else if ((cardPeekIsBlack ^ cardMoveIsBlack) 
            && (cardToMove.value == (cardPeek.value - 1))) 
        {
            Debug.Log("1) Valid: Moving Card " 
                + cardToMove.name + " to Card " + cardPeek.name);

            return true;
        }

        Debug.Log("Invalid sequence: Card " 
            + cardToMove.suit + cardToMove.value + " to Card " 
            + cardPeek.suit + cardPeek.value);
        Debug.Log("Clicked on Foundation? " 
            + (cardPeek.isInFoundationPile ? "Yes" : "No") 
            + "; Same suit? " 
            + (cardPeek.suit == cardToMove.suit ? "Yes" : "No") 
            + "; Selected card value = " 
            + cardToMove.value);

        return false;
    }

    private void BuildSequence(GameObject clickedCard)
    {
        CardItem cardToMove = _selectedCardGO.GetComponent<CardItem>();
        CardItem cardPeek = clickedCard.GetComponent<CardItem>();
        float yOffset = 0.3f;
        float zOffset = 0.02f;

        // Set yOffset to 0 for empty and Foundation piles
        if (cardPeek.isInFoundationPile
            || (!cardPeek.isInFoundationPile && cardToMove.value == 13))
            yOffset = 0;

        cardToMove.transform.position = new Vector3(
            clickedCard.transform.position.x, 
            clickedCard.transform.position.y - yOffset, 
            clickedCard.transform.position.z - zOffset);
        
        // Set selectedCard as the Child of clickedCard
        // (makes it easier to move sequences as they grow)
        cardToMove.transform.parent = clickedCard.transform;

        // Leaving Talon
        if (cardToMove.isInTalonPile)
        {
            if (_solitaireManager
                .talonPileList
                .Remove(cardToMove.name))
            {
                Debug.Log("2) Removing Card " + cardToMove.name
                    + " from Talon List");

                cardToMove.isInTalonPile = false;
            }
            else
            {
                Debug.LogError("Failed to remove Card " + cardToMove.name
                    + " from Talon List");
            }
        }
        // Leaving Foundation
        else if (cardToMove.isInFoundationPile
            && !cardPeek.isInFoundationPile)
        {
            Debug.Log("2) Removing Card " + cardToMove.name
                + " from Foundation " + cardToMove.pileIndex);

            cardToMove.isInFoundationPile = false;
        }

        // Moving to Foundation
        if (cardPeek.isInFoundationPile)
            cardToMove.isInFoundationPile = true;

        // Leaving a Tableau pile: (Attempt to) Remove from list
        // if it was part of the initial pile
        if (_solitaireManager
            .tableauPilesList[cardToMove.pileIndex]
            .Remove(cardToMove.name))
        {
            Debug.Log("2) Removing Card " + cardToMove.name
                + " from Tableau List " + cardToMove.pileIndex);
        }

        // Move is done; update pilePos and reset selectedCard
        cardToMove.pileIndex = cardPeek.pileIndex;

        _spriteController.ToggleHighlight(_selectedCardGO);
        _selectedCardGO = this.gameObject;
    }
}
