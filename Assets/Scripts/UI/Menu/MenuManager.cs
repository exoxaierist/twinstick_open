using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Image fader;
    public GameObject mainMenu;
    public GameObject settings;

    public GameObject startButton;

    private void Start()
    {
        fader.gameObject.SetActive(true);
        fader.DOFade(0, 1);
        OpenMainMenu();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
        settings.GetComponentInChildren<MenuSettings>().OnOpen();
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
