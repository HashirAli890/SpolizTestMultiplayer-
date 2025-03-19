using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    public Button LoadButton;
    public Scenes SceneToLoad;
    public TMP_InputField UserNameField;
    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(EventHandler.PlayerNamePref))) 
        {
            UserNameField.text = PlayerPrefs.GetString(EventHandler.PlayerNamePref);
            LoadButton.gameObject.SetActive(true);
        }
        LoadButton.onClick.AddListener(()=> 
        {
            PlayerPrefs.SetString(EventHandler.PlayerNamePref, UserNameField.text);
            EventHandler.LoadScene?.Invoke(SceneToLoad);
        });
    }
    public void OnEnterUserName() 
    {
        if (!string.IsNullOrEmpty(UserNameField.text))
        {
            LoadButton.gameObject.SetActive(true);
        }
        else 
        {
            LoadButton.gameObject.SetActive(false);
        }
    }
    public void OnValueChange() 
    {
        LoadButton.gameObject.SetActive(false);
    }
}
