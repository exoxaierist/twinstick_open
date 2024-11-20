using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunResult : MonoBehaviour
{
    public TextMeshProUGUI runCount;

    public void ShowResult()
    {
        StringBuilder builder = new();
        builder.Append(Locale.Get("UI_KILLEDBY"));
        builder.Append(Player.main.deathBlow.attackerName);
        builder.Append("\n");
        builder.Append("\n");
        builder.Append(Locale.Get("UI_CHAMBERSCLEARED"));
        builder.Append(LevelManager.roomNumber - 1);
        builder.Append("\n");
        builder.Append("\n");
        builder.Append(Locale.Get("UI_PERKSCARRIED"));
        builder.Append("\n");
        for (int i = 0; i < Player.perks.Count; i++)
        {
            if(i!=0) builder.Append(", ");
            builder.Append(Player.perks[i].name);
            builder.Append("\n");
        }
        builder.Append("\n");
         
        runCount.text = builder.ToString();
        gameObject.SetActive(true);
    }

    public void FinishResult()
    {
        UIManager.main.TopFadeIn(0.5f);
        this.Delay(0.6f, () => 
        {
            DOTween.Clear();
            SceneManager.LoadScene("Menu");
        });
    }

    private void Update()
    {
        if (!enabled) return;
        if (Input.GetKeyDown(KeyCode.Return)) FinishResult();
    }
}
