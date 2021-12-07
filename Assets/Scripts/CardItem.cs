using UnityEngine;

public class CardItem : MonoBehaviour
{
    public string Suit { get; private set; }
    public int Value   { get; private set; }
    public int PileIndex;

    public bool IsFaceUp { get; private set; }
    public bool IsInTalonPile { get; private set; }
    public bool IsInFoundationPile { get; private set; }

    private SpriteController _spriteController;

    void Awake()
    {
        _spriteController = GetComponent<SpriteController>();
    }

    void Start()
    {
        if (CompareTag("Card"))
        {
            Suit = transform.name[0].ToString();

            char c = transform.name[1];

            if (c == 'A')
                Value = 1;
            else if (c == '1')
                Value = 10;
            else if (c == 'J')
                Value = 11;
            else if (c == 'Q')
                Value = 12;
            else if (c == 'K')
                Value = 13;
            else
                Value = (int)char.GetNumericValue(c);

            IsInFoundationPile = false;
        }
        if (CompareTag("Foundation"))
        {
            Suit = null;
            Value = 0;

            IsInFoundationPile = true;
        }
        else if (CompareTag("Tableau"))
        {
            Suit = null;
            Value = 14;

            IsInFoundationPile = false;
        }
    }
    
    public void InstantiatedInTalon() 
    {
        IsFaceUp = true;
        IsInTalonPile = true;
        _spriteController.ToggleSprite();
    }

    public void SetFaceUp()
    {
        IsFaceUp = true;
        _spriteController.ToggleSprite();
    }

    public void HasLeftTalonPile() 
    {
        IsInTalonPile = false;
    }

    public void UpdateFoundationStatus() 
    {
        // Didn't want to set this as a toggle since a card can move between Foundation piles
        Transform t = this.gameObject.transform;
        bool childOfFoundation = false;

        while (t.parent != null) 
        {
            if (t.parent.CompareTag("Foundation"))
                childOfFoundation = true;

            t = t.parent.transform;
        }

        if (childOfFoundation)
            IsInFoundationPile = true;
        else
            IsInFoundationPile = false;
    }
}
