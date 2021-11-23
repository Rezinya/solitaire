using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        OnMouseClick();
    }

    private void OnMouseClick()
    {
        // We want the player to only interact with the top-most card for each pile
        // To do this, we utilize Raycasts
        if (Input.GetMouseButton(0))
        {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(
                Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (hit.collider.CompareTag("Tableau"))
                {
                    Debug.Log("Player has clicked on a tableau pile.");
                    onTableauHit();
                }
                else if (hit.collider.CompareTag("Foundation"))
                {
                    Debug.Log("Player has clicked on a foundation pile.");
                    onFoundationHit();
                }
                else if (hit.collider.CompareTag("Stock"))
                {
                    Debug.Log("Player has clicked on the stock (or hand) pile.");
                    onStockHit();
                }
                else if (hit.collider.CompareTag("Talon"))
                {
                    Debug.Log("Player has clicked on the talon (or waste) pile.");
                    onTalonHit();
                }
            }
        }
    }

    private void onTableauHit()
    {
        //
    }

    private void onFoundationHit()
    {
        //
    }

    private void onStockHit()
    {
        //
    }

    private void onTalonHit()
    {
        //
    }
}
