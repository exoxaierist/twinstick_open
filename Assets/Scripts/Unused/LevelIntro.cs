using UnityEngine;
using TMPro;
using DG.Tweening;

public class LevelIntro : MonoBehaviour
{
    private static LevelIntro main;

    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;

    private CanvasGroup group;

    private void Awake()
    {
        main = this;
        group = GetComponent<CanvasGroup>();
    }

    public static void Show()
    {
        main.group.DOFade(1, 0.5f);

        switch (LevelManager.level)
        {
            case 1:
                main.text.text = "Level One";
                main.text2.text = "Shallow Depth";
                break;
            case 2:
                main.text.text = "Level Two";
                main.text2.text = "Cavity";
                break;
        }

        main.Delay(1.5f, () => main.group.DOFade(0, 0.5f));
    }
}
