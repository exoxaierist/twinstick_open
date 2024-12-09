using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderExtended : Slider
{
    public override void OnSelect(BaseEventData eventData)
    {
        SoundSystem.Play(SoundSystem.UI_HOVER);
        base.OnSelect(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        SoundSystem.Play(SoundSystem.UI_HOVER);
        base.OnPointerEnter(eventData);
    }

    protected override void Set(float input, bool sendCallback = true)
    {
        SoundSystem.Play(SoundSystem.UI_SUBMIT);
        base.Set(input, sendCallback);
    }
}
