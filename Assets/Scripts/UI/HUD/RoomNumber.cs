using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomNumber : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    private MetaRoom prevRoom;

    private void Start()
    {
        LevelManager.main.onRoomChange += OnRoomChange;   
    }

    private void OnRoomChange(MetaRoomInfo info)
    {
        if (prevRoom != null) prevRoom.onRoomClear -= OnRoomClear;
        prevRoom = info.metaRoom;
        info.metaRoom.onRoomClear += OnRoomClear;
        if(info.type == RoomType.Combat)
        {
            if (!info.metaRoom.isCleared)
            {
                image.DOFade(1, 0.2f).SetUpdate(true);
            }
            text.text = Locale.Get("UI_CHAMBER") +" "+info.roomNumber;
            text.DOFade(1, 0.2f).SetUpdate(true);
        }
        else
        {
            image.DOFade(0, 0.2f).SetUpdate(true);
            text.DOFade(0, 0.2f).SetUpdate(true);
        }
    }

    private void OnRoomClear()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.1f).SetUpdate(true));
        sequence.AppendInterval(0.1f);
        sequence.Append(image.DOFade(1, 0.1f).SetUpdate(true));
        sequence.AppendInterval(0.1f);
        sequence.Append(image.DOFade(0, 0.1f).SetUpdate(true));
        sequence.AppendInterval(0.1f);
        sequence.Append(image.DOFade(1, 0.1f).SetUpdate(true));
        sequence.AppendInterval(0.1f);
        sequence.Append(image.DOFade(0, 0.1f).SetUpdate(true));
        sequence.Play();
    }
}
