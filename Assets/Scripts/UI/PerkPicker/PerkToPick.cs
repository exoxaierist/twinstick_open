using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkToPick : Selectable, ISubmitHandler
{
    public Perk perk;
    public string perkID;
    public int index;
    public PerkDescriptionUI infoBox;
    public Image selected;
    public Image hovered;
    public Image perkImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descText;

    private CanvasGroup group;
    private PerkPicker picker;

    override public void OnPointerEnter(PointerEventData eventData) => Select();
    override public void OnPointerDown(PointerEventData eventData) => Pick();
    override public void OnSelect(BaseEventData eventData) => ShowHover();
    override public void OnDeselect(BaseEventData eventData) => HideHover();
    public void OnSubmit(BaseEventData eventData) => Pick();

    private void ShowHover()
    {
        hovered.gameObject.SetActive(true);
    }

    private void HideHover() 
    {
        hovered.gameObject.SetActive(false);
    }

    private void Pick() 
    {
        Select();
        selected.gameObject.SetActive(true);
        picker.SelectPerk(this);
    }

    public void Unpick()
    {
        selected.gameObject.SetActive(false);
    }

    public void Set(string _perkID, PerkPicker _picker, int _index)
    {
        perkID = _perkID;
        picker = _picker;
        index = _index;
        perk = Perk.Get(perkID);

        perkImage.sprite = perk.sprite;
        nameText.text = perk.name;
        descText.text = perk.description;
        if (Player.HasPerk(_perkID)) levelText.text = (Player.GetExistingPerk(_perkID).level+1) + "/" + perk.maxLevel;
        else levelText.text = "1/" + perk.maxLevel;

        //appear animation
        ((RectTransform)transform).DOAnchorPosY(30, 0.3f).From().SetDelay(_index * 0.08f + 0.2f).SetUpdate(true);
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.DOFade(1, 0.2f).SetDelay(_index * 0.08f + 0.2f).SetUpdate(true);
    }

    public void FadeRemove()
    {
        this.DelayRealtime(0.3f, () => Destroy(gameObject));
    }
}
