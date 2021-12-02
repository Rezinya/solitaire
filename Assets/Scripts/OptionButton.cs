using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionButton : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject newGameButton;
    public GameObject optionsButton;
    public GameObject closeOptionsButton;

    private void Awake()
    {
        optionsPanel.SetActive(false);

        // Hiding Options Button for now...
        optionsButton.SetActive(false);
    }

    public void OnNewGameClick() 
    {
        Debug.Log("Starting new game...");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnOptionsClick() 
    {
        optionsPanel.SetActive(true);

        newGameButton.SetActive(false);
        optionsButton.SetActive(false);
    }
}
