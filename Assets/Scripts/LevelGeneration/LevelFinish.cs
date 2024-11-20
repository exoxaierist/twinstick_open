using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    public static LevelFinish main;
    public static bool isEnabled = false;

    public Sprite enabledSprite;

    private void Awake()
    {
        main = this;
        isEnabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) FinishLevel();
    }

    public static void EnableFinish() { if (main != null) main.InternalEnableFinish(); }
    private void InternalEnableFinish()
    {
        if (isEnabled) return;
        isEnabled = true;

        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {   
            sprite.sprite = enabledSprite;
        }
    }

    private void FinishLevel()
    {
        if (!isEnabled) return;
        GameManager.main.FinishLevel();
    }
}
