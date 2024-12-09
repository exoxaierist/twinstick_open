using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu main;

    public static void Open()
    {
        if (main == null) return;
        GameManager.PauseGame();
        UIManager.main.ShowOverlayBackground();
        UIManager.main.overlayManager.UpdatePerks();
        main.gameObject.SetActive(true);
        main.group.DOKill();
        main.group.DOFade(1, 0.2f).SetUpdate(true);
        EventSystem.current.SetSelectedGameObject(main.resumeButton);
    }
    public static void Close()
    {
        GameManager.ResumeGame();
        UIManager.main.HideOverlayBackground();
        main.BackToPauseMenu();
        main.group.DOKill();
        main.group.DOFade(0, 0.2f).SetUpdate(true).OnComplete(()=> main.gameObject.SetActive(false));
    }

    public CanvasGroup group;
    public GameObject resumeButton;
    public GameObject mainGroup;
    public GameObject settingsGroup;
    public GameObject howtoGroup;
    public GameObject howtoBackBtn;

    public void OpenHowTo()
    {
        mainGroup.SetActive(false);
        howtoGroup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(howtoBackBtn);
    } 

    public void OpenSettings()
    {
        mainGroup.SetActive(false);
        settingsGroup.SetActive(true);
        settingsGroup.GetComponentInChildren<MenuSettings>().OnOpen();
    }

    public void Restart()
    {
        Player.Reset();
        DOTween.KillAll();
        GameManager.ResumeGame();
        SceneManager.LoadScene("Game");
    }

    public void ToMainMenu()
    {
        Player.Reset();
        DOTween.KillAll();
        GameManager.ResumeGame();
        SceneManager.LoadScene("Menu");
    }

    public void BackToPauseMenu()
    {
        mainGroup.SetActive(true);
        settingsGroup.SetActive(false);
        howtoGroup.SetActive(false);
        EventSystem.current.SetSelectedGameObject(main.resumeButton);
    }
}
