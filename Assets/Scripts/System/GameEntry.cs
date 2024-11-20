using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntry : MonoBehaviour
{
    public const int VERSION = 1;
    private void Start()
    {
        Settings.LoadSettings();
        Settings.ApplySettings();
        SceneManager.LoadScene("Menu");
    }
}
