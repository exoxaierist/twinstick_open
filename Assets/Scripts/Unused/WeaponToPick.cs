using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponToPick : MonoBehaviour, IPointerDownHandler
{
    private WeaponPicker picker;
    private RectTransform rect;
    public WeaponInfo info;
    private int index;
    private float posX;

    public void OnPointerDown(PointerEventData eventData) => picker.Select(info, posX, index);

    public void SelectByIndex(int newIndex) { if (index == newIndex) picker.Select(info, posX, index); }

    public void Set(WeaponPicker _picker, WeaponInfo _info)
    {
        picker = _picker;
        info = _info;
        index = -1;

        Image image = GetComponent<Image>();
        image.sprite = info.sprite;
        if (!info.unlocked) image.color = Utility.GetGray(0.2f);
        image.SetNativeSize();

        rect = transform as RectTransform;
        rect.localScale = new(2f, 2f, 1);
        posX = rect.anchoredPosition.y;
        this.DelayFrame(()=>
        {
            posX = rect.anchoredPosition.y;
            index = transform.GetSiblingIndex();
            if (index == 0) picker.Select(info, posX, index);
        });

    }

    public void OnSelectChange(int newIndex)
    {
        if(newIndex == index)
        {
            rect.DOAnchorPosX(60, 0.3f)
                .SetEase(Ease.OutCubic);
            return;
        }
        rect.DOAnchorPosX(0, 0.3f)
            .SetEase(Ease.OutCubic);
    }
}
