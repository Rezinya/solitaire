using UnityEngine;

public class StockSpriteController : MonoBehaviour
{
    private SolitaireManager _solitaireManager;
    private SpriteRenderer _spriteRenderer;
    
    public Sprite ResetPile;
    public Sprite CardBack;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _solitaireManager = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent<SolitaireManager>();
    }

    public void ToggleSprite()
    {
        if (_solitaireManager.IsStockEmpty)
            _spriteRenderer.sprite = ResetPile;
        else
            _spriteRenderer.sprite = CardBack;
    }
}
