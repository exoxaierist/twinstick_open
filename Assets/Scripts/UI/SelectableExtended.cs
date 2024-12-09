using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableExtended : Selectable
{
    public override void OnSelect(BaseEventData eventData)
    {
        SoundSystem.Play(SoundSystem.UI_HOVER);
        base.OnSelect(eventData);
    }
}
