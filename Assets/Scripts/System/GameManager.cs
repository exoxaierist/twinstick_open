using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public enum GameMode
{
    Stage,
    Lobby, 
}

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    public static bool isPaused = false;

    //references
    public Material playerMat;
    public Material enemyMat;

    [Header("Game Mode")]
    public GameMode gameMode;

    [Header("System")]
    public int targetFramerate = 60;

    private InputDevice lastInputDevice = null;
    public bool isUsingController = false;

    private void Awake()
    {
        main = this;
        Application.targetFrameRate = targetFramerate;
        Cursor.visible = false;

        InputSystem.onAnyButtonPress.Call((control) =>
        {
            if (control.device is Mouse) isUsingController = false;
            else if (control.device is Gamepad) isUsingController = true;
        });
    }

    public void OnStart()
    {
        StartLevel();
    }

    public static void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        SoundSystem.FadeMusic();
        if(Player.main!=null)Player.main.Sleep(true);
    }

    public static void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        SoundSystem.UnfadeMusic();
        if(Player.main!=null)Player.main.Sleep(false);
    }

    private void StartLevel()
    {
        DOTween.KillAll();
        UIManager.main.TopFadeOut(0.5f);
        UIManager.main.FadeIn(0.01f);
        PlayerStats.Reset();
        PersistentPlayerState.main.playerWeapon = "P3000";
        
        this.Delay(0.2f, () => PerkPicker.Pick(() =>
        {
            LevelManager.main.StartLevel(() =>
            {
                UIManager.main.FadeOut(1);
                PlayerStart.SpawnCamera();
                this.Delay(0.7f, () => PlayerStart.SpawnPlayer());
            });
        }));
    }

    /*public void EnterGame()
    {
        UIManager.main.TopFadeIn(0.3f);
        this.Delay(0.3f, () =>
        {
            PlayerStart.DespawnPlayer();
            PersistentPlayerState.main.playerWeapon = "BasicPistol";
            PersistentPlayerState.main.doSpawnPlayerWeapon = true;
            gameMode = GameMode.Stage;
            DOTween.KillAll();

            SceneManager.LoadScene("Game");
        });
    }*/

    /*private void StartLobby()
    {
        UIManager.main.TopFadeOut(0.5f);
        PersistentPlayerState.main.doSpawnPlayerWeapon = false;

        PlayerStart.SpawnPlayerLobby();
    }*/

    /*public void FinishLevel()
    {
        //animation?
        //fade in
        UIManager.main.HideHud();
        UIManager.main.HideOverlay();
        UIManager.main.TopFadeIn(0.6f);
        this.Delay(0.6f,()=>LevelManager.main.EndLevel());
        this.Delay(0.7f,()=>StartLevel());
    }*/
}
