using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;

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
        StartCoroutine("LoadSceneWithFadeEffect");
    }

    public void ContinueGame()
    {
        StartCoroutine("LoadSceneWithFadeEffect");
    }

    public void ExitGame()
    {
        Debug.Log("Exited Game");
        // Application.Quit();
    }

    IEnumerator LoadSceneWithFadeEffect()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }


}
