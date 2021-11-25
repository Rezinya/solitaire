using System.Collections;
using System.Collections.Generic;
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
        

        // Initialize highlightedCard to SolitaireGame
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
            if (hit.collider.CompareTag("Tableau"))
                OnTableauClick();
            else if (hit.collider.CompareTag("Foundation"))
                OnFoundationClick();
            else if (hit.collider.CompareTag("Stock"))
                OnStockClick();
            else if (hit.collider.CompareTag("Card"))
                OnCardClick(hit.collider.gameObject); 
        }
    }

    private void OnTableauClick()
    {
        Debug.Log("Clicked on a Tableau pile.");
    }

    private void OnFoundationClick()
    {
        Debug.Log("Clicked on Foundation pile.");
    }

    private void OnStockClick()
    {
        Debug.Log("Clicked on Stock pile.");
        _solitaireManager.DealTalonByThree();
        _stockSpriteController.ToggleSprite();
    }

    private void OnCardClick(GameObject clickedCard)
    {
        Debug.Log("Clicked on Card: " + clickedCard.name);
    }
}
