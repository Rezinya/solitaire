using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionButton : MonoBehaviour
{
    public GameObject OptionsPanel;
    public GameObject NewGameButton;
    public GameObject OptionsButton;

    void Start()
    {
        OptionsPanel.SetActive(false);

        // Hide the Options Button for now...
        OptionsButton.SetActive(false);
    }

    public void OnNewGameClick() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnOptionsClick() 
    {
        OptionsPanel.SetActive(true);

        NewGameButton.SetActive(false);
        OptionsButton.SetActive(false);
    }
}
