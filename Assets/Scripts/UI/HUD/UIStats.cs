using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStats : MonoBehaviour
{
    public static UIStats main;

    public Transform shieldGroup;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI hpText;
    public Image hpFiller;
    public Image hpFillerEffect;
    public Sprite shieldBreakSprite;

    [HideInInspector] public Hp targetedHp;

    private int hp;
    private int maxHp;
    private int tweenedHp;
    private Tweener coinCountTweener;
    private Tweener hpTweener;

    private void Awake()
    {
        if (main == null) main = this;
        else Destroy(gameObject);
    }

    public static void Hide()
    {
        main.gameObject.SetActive(false);
    }

    public static void SetTarget(Hp targetHp)
    {
        main.targetedHp = targetHp;
        main.targetedHp.onDamage += main.UpdateHp;
        main.targetedHp.onHeal += main.UpdateHp;
        main.UpdateHp();
    }

    public void UpdateHp()
    {
        hp = targetedHp.health;
        maxHp = targetedHp.maxHealth;
        hpTweener.Kill();
        hpTweener = DOTween.To(() => tweenedHp, x => { tweenedHp = x; hpText.text = x + "/" + maxHp; }, hp, 0.5f);
        SetFillerAmount();
    }

    public void SetCoin(int amount)
    {
        PrivateSetCoin(amount);
    }

    private void PrivateSetCoin(int amount)
    {
        coinCountTweener.Kill();
        coinCountTweener = DOTween.To(() => int.Parse(coinText.text), x => coinText.text = "" + x, amount, 0.5f);
    }

    public void SetShield(int count)
    {
        int childCount = main.shieldGroup.childCount;
        if (count == childCount) return;
        if (count > childCount)
        {
            for (int i = 0; i < count-childCount; i++)
            {
                main.CreateShield();
            }
        }
        else
        {
            for (int i = 0; i < childCount - count; i++)
            {
                main.RemoveShield();
            }
        }
    }

    private void SetFillerAmount()
    {
        hpFiller.DOFillAmount(hp / (float)maxHp, 0.05f).SetEase(Ease.OutQuad);
        hpFillerEffect.DOFillAmount(hp / (float)maxHp, 0.2f).SetEase(Ease.InOutQuad).SetDelay(0.15f);
    }

    private void CreateShield()
    {
        GameObject instance = Instantiate(Prefab.Get("ShieldIcon"));
        instance.SetActive(true);
        instance.transform.SetParent(shieldGroup);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)shieldGroup.transform);
        instance.transform.DOLocalMoveY(instance.transform.localPosition.y + 10, 0.1f).From();
    }

    private void RemoveShield()
    {
        Image icon = shieldGroup.GetChild(shieldGroup.childCount - 1).GetComponent<Image>();

        icon.sprite = shieldBreakSprite;
        icon.color = new(0.305f, 0.305f, 0.305f);

        this.Delay(0.05f, () => icon.enabled = false);
        this.Delay(0.1f, () => icon.enabled = true);
        this.Delay(0.15f, () => icon.enabled = false);
        this.Delay(0.2f, () => icon.enabled = true);
        this.Delay(0.25f, () => Destroy(icon.gameObject));
    }
}
