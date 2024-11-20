using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionUI : MonoBehaviour
{
    public static GameObjectPool pool = new("DescriptionUI");
    //ui that exist in world
    public static DescriptionUI GetWorldSpace(Vector2 pos, ItemDisplayInfo info, string optionText = "")
    {
        DescriptionUI instance = pool.Get().GetComponent<DescriptionUI>();
        instance.SetWorldSpace(pos, info, optionText);
        return instance;
    }
    public static DescriptionUI GetScreenSpace(Transform parent,Vector2 pos,ItemDisplayInfo info, string optionText="")
    {
        DescriptionUI instance = pool.Get().GetComponent<DescriptionUI>();
        instance.SetScreenSpace(parent, pos, info, optionText);
        return instance;
    }
    public static void Release(DescriptionUI ui)
    {
        ui.Hide(()=> pool.Release(ui.gameObject));
    }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI optionText;

    private bool isWorldSpace = false;
    private Vector2 position;
    private Vector3 shake;
    private Vector3 offset;

    private Tweener shakeTween;
    private CanvasGroup canvasGroup;

    private void Update()
    {
        if (!isWorldSpace) return;
        Vector3 halfScreen = new(Screen.width*0.5f, Screen.height*0.5f, 0);
        transform.position = (Camera.main.WorldToScreenPoint(position)-halfScreen)*1.05f + halfScreen + shake + offset;
    }

    private void SetWorldSpace(Vector2 pos, ItemDisplayInfo info, string _optionText)
    {
        isWorldSpace = true;
        canvasGroup = GetComponent<CanvasGroup>();
        transform.SetParent(UIManager.main.shopUIParent);
        position = pos;
        offset = new(10, -10);
        transform.position = Camera.main.WorldToScreenPoint(position) + offset;
        transform.localScale = Vector3.one;
        Show();

        nameText.text = info.name;
        descText.text = info.description;
        optionText.text = _optionText;

        descText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp(descText.preferredWidth,100,400));

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void SetScreenSpace(Transform parent, Vector2 pos, ItemDisplayInfo info, string _optionText)
    {
        isWorldSpace = false;
        canvasGroup = GetComponent<CanvasGroup>();
        transform.localScale = Vector3.one;
        transform.SetParent(parent);
        transform.localPosition = pos;
        Show();

        nameText.text = info.name;
        descText.text = info.description;
        optionText.text = _optionText;

        descText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp(descText.preferredWidth, 100, 400));

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void Shake()
    {
        shakeTween.Rewind();
        shakeTween = DOTween.Shake(() => Vector3.zero, x => shake = x, 0.2f, new Vector3(3, 3, 0), 40,90,false);
    }

    public void Hide(Action onFinish = null)
    {
        canvasGroup.DOFade(0, 0.1f)
            .OnComplete(()=>onFinish?.Invoke())
            .SetUpdate(true);
    }

    private void Show()
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.1f)
            .SetUpdate(true);
    }
}
