using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonExtended : Button
{
    public override void OnSelect(BaseEventData eventData)
    {
        SoundSystem.Play(SoundSystem.UI_HOVER);
        base.OnSelect(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(interactable)SoundSystem.Play(SoundSystem.UI_HOVER);
        base.OnPointerEnter(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        SoundSystem.Play(SoundSystem.UI_SUBMIT);
        base.OnSubmit(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (interactable) SoundSystem.Play(SoundSystem.UI_SUBMIT);
        base.OnPointerDown(eventData);
    }
}
