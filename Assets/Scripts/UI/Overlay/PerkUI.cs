using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkUI : Selectable,ISelectHandler,IDeselectHandler
{
    [SerializeField] private Transform levelWrapper;
    public Image perkImage;

    private Vector2 origin;
    private Vector2 sway;
    private Vector2 hover;
    private Vector2 random;
    private Perk perk;
    private OverlayManager manager;

    //private void Update() => transform.position = origin + sway * 10 + hover + random;

    public override void OnSelect(BaseEventData eventData)=>OnSelect();
    public override void OnDeselect(BaseEventData eventData)=>OnDeselect();
    override public void OnPointerEnter(PointerEventData eventData)=>Select();

    private void OnSelect()
    {
        transform.DOKill();
        transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.2f)
            .SetUpdate(true);
        manager.perkInfoBox.gameObject.SetActive(true);
        manager.perkName.text = perk.name;
        manager.perkDesc.text = perk.description;
        manager.perkLevel.text = perk.level + "/" + perk.maxLevel;
        manager.perkInfoBox.transform.position = transform.position;
        LayoutRebuilder.ForceRebuildLayoutImmediate(manager.perkInfoBox as RectTransform);
    }
    private void OnDeselect()
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, 0.2f)
            .SetUpdate(true);
        manager.perkName.text = "";
        manager.perkDesc.text = "";
        manager.perkLevel.text = "";
        manager.perkInfoBox.gameObject.SetActive(false);
    }

    public void Set(Perk _perk, OverlayManager _manager)
    {
        perk = _perk;
        manager = _manager;
        perkImage.sprite = SpriteLib.Get(perk.ID);
        SetPerkLevel(perk.level, perk.maxLevel);
    }

    private void SetPerkLevel(int level, int maxLevel)
    {
        int childCount = levelWrapper.childCount;
        for (int i = 0; i < childCount; i++) Destroy(levelWrapper.GetChild(i).gameObject);
        for (int i = 0; i < level; i++)
        {
            GameObject instance = Instantiate(Prefab.Get("PerkLevelFull"));
            instance.transform.SetParent(levelWrapper);
            instance.transform.localScale = Vector3.one;
        }
        for (int i = level; i < maxLevel; i++)
        {
            GameObject instance = Instantiate(Prefab.Get("PerkLevelEmpty"));
            instance.transform.SetParent(levelWrapper);
            instance.transform.localScale = Vector3.one;
        }
    }
}
