using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveDial : MonoBehaviour
{
    public Image dial;
    public TextMeshProUGUI text;
    private MetaRoom prevMetaRoom;

    private void Start()
    {
        LevelManager.main.onRoomChange += OnRoomChange;
    }

    private void OnRoomChange(MetaRoomInfo info)
    {
        if(prevMetaRoom != null) prevMetaRoom.onWaveStart -= OnWaveStart;
        if (info.type != RoomType.Combat || info.metaRoom.isCleared) return;
        info.metaRoom.onWaveStart += OnWaveStart;
        prevMetaRoom = info.metaRoom;

        text.DOFade(1, 0.5f).SetDelay(1);
        dial.DOFillAmount(1, 2).OnComplete(()=>dial.fillAmount = 0);
        text.text = Locale.Get("UI_WAVE") + " 1/"+prevMetaRoom.waveCount;
    }

    private void OnWaveStart()
    {
        int wave = prevMetaRoom.waveCount - prevMetaRoom.currentWaveCount;
        text.text = Locale.Get("UI_WAVE") +" " + wave + '/' + prevMetaRoom.waveCount;
        if (prevMetaRoom.currentWaveCount == 0)
        {
            dial.fillAmount = 0;
            text.DOFade(0, 1).SetDelay(5);
        }
        else
        {
            dial.DOFillAmount(1, PlayerStats.waveInterval).OnComplete(()=>dial.fillAmount = 0);
        }
    }
}
