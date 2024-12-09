using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHP : MonoBehaviour
{
    public static BossHP main;
    public static GameObjectPool pool = new("BossHP");

    public static void Spawn(string bossId, Hp hp)
    {
        BossHP instance = pool.Get().GetComponent<BossHP>();
        instance.transform.SetParent(UIManager.main.bossHpParent);
        instance.transform.localPosition = Vector3.zero;
        instance.linkedHp = hp;
        instance.bossId = bossId;
        instance.Set();
    }

    public TextMeshProUGUI bossNameText;
    public Image hpBarRed;
    public Image hpBarWhite;
    private CanvasGroup group;

    private Hp linkedHp;
    private string bossId;

    private bool removing = false;

    private void Awake()
    {
        main = this;
    }

    public void Set()
    {
        group = GetComponent<CanvasGroup>();

        bossNameText.text = Locale.Get(bossId);
        linkedHp.onDamage += UpdateHp;
        linkedHp.onHeal += UpdateHp;
        group.DOKill();
        group.DOFade(1, 0.5f).SetUpdate(true);
        UpdateHp();
    }

    public void Remove()
    {
        if (removing) return;
        removing = true;
        group.DOKill();
        group.DOFade(0, 0.5f).SetUpdate(true).SetDelay(1);
        this.DelayRealtime(1.5f, RemoveImmediate);
    }

    private void RemoveImmediate()
    {
        group.alpha = 0;
        linkedHp.onDamage -= UpdateHp;
        linkedHp.onHeal -= UpdateHp;
        removing = false;
        pool.Release(gameObject);
    }

    private void UpdateHp()
    {
        hpBarRed.DOFillAmount(linkedHp.health / (float)linkedHp.maxHealth, 0.05f).SetEase(Ease.OutQuad);
        hpBarWhite.DOFillAmount(linkedHp.health / (float)linkedHp.maxHealth, 0.1f).SetEase(Ease.InOutQuad).SetDelay(0.1f);
        if (linkedHp.health == 0) Remove();
    }
}
