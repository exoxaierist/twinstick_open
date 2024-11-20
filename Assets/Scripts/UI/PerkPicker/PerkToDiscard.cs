using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkToDiscard : Selectable, ISubmitHandler
{
    public Image perkImage;
    public Image selected;
    public Image hovered;
    public Transform levelWrapper;

    public Perk perk;
    private PerkPicker picker;

    public override void OnPointerEnter(PointerEventData eventData) => Select();
    public override void OnPointerDown(PointerEventData eventData) => Pick();
    public override void OnSelect(BaseEventData eventData) => ShowHover();
    public override void OnDeselect(BaseEventData eventData) => HideHover();
    public void OnSubmit(BaseEventData eventData) => Pick();

    private void ShowHover() 
    {
        hovered.gameObject.SetActive(true);
        picker.perkNameText.text = perk.name;
        picker.perkDescText.text = perk.description;
        picker.perkLevelText.text = perk.level + "/" + perk.maxLevel;
    }

    private void HideHover() 
    {
        hovered.gameObject.SetActive(false);
    }

    private void Pick()
    {
        Select();
        if (!picker.discardMode) return;
        selected.gameObject.SetActive(true);
        picker.SelectDiscard(this);
    }

    public void Unpick()
    {
        selected.gameObject.SetActive(false);
    }

    public void Set(int _index, Perk _perk, PerkPicker _picker)
    {
        perk = _perk;
        picker = _picker;
        picker.onDiscardDisabled += Unpick;
        perkImage.sprite = SpriteLib.Get(perk.ID);
        SetPerkLevel(perk.level, perk.maxLevel);
    }

    public void FadeRemove()
    {
        this.Delay(0.3f, () => Destroy(gameObject));
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
