
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public enum Scenes 
{
    LoadingScene, MainMenu, GamePlay
}
public class SceneLoader : MonoBehaviour
{
    public GameObject LoadingScreen;
    public TMP_Text LoadingText;
    private void Awake()
    {
        EventHandler.LoadScene += Load;
        EventHandler.LoadLoadingScreen += ActivateLoadingScreen;
        EventHandler.UnloadLoadingScreen += DisableLoading;
    }

    private void Load(Scenes scene)
    {
      SceneManager.LoadScene(scene.ToString());
    }
    private void OnDisable()
    {
        EventHandler.LoadScene -= Load;
        EventHandler.LoadLoadingScreen -= ActivateLoadingScreen;
        EventHandler.UnloadLoadingScreen -= DisableLoading;
    }
    public void ActivateLoadingScreen(string LoadingMessage) 
    {
        if (LoadingScreen)
        {
            LoadingScreen.SetActive(true);
            LoadingText.text = LoadingMessage;
        }
    }
    public void DisableLoading() 
    {
        if (LoadingScreen)
        {
            LoadingScreen.SetActive(false);
            LoadingText.text = null;
        }
    }
}
