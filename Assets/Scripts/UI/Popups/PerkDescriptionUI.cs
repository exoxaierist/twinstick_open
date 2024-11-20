using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PerkDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI levelText;
    private CanvasGroup group;

    public static PerkDescriptionUI Hover(Perk perk)
    {
        PerkDescriptionUI instance = Instantiate(Prefab.Get("PerkDescriptionUI")).GetComponent<PerkDescriptionUI>();
        instance.transform.SetParent(UIManager.mouseFollow);
        instance.transform.localScale = Vector3.one;
        instance.transform.localPosition = new(10, -10);
        instance.Set(perk);
        return instance;
    }

    public void EndHover()
    {
        Destroy(gameObject);
    }

    public void Set(Perk perk, bool raycastTarget = false)
    {
        if (group == null) group = GetComponent<CanvasGroup>();
        if (raycastTarget)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        nameText.text = perk.name;
        descText.text = perk.description;
        levelText.text = perk.level + "/" + perk.maxLevel;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
