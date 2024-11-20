using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    private void Awake()
    {
        //LevelManager.Init();
        //Player.Init();
    }

    private void Start()
    {
        //if (GameManager.main == null) return;
        
        GameManager.main.OnStart();
        Destroy(gameObject);
    }
}
