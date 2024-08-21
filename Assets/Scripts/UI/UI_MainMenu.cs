using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;

    private void Start()
    {
        if (SaveManager.instance.HasSavedData())
            continueButton.SetActive(true);
        else
            continueButton.SetActive(false);
    }
    
    public void NewGame()
    {
        SaveManager.instance.DeleteSavedData();
        SceneManager.LoadScene(sceneName);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(sceneName);
        
    }

    public void ExitGame()
    {
        Debug.Log("Exited Game");
        // Application.Quit();
    }
}
