using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public static GameObjectPool pool = new("DamageIndicator");

    public static void Show(Vector2 worldPos, AttackInfo info)
    {
        GameObject instance = pool.Get();
        TextMeshPro text = instance.GetComponent<TextMeshPro>();

        Vector3 pos = worldPos;
        pos.z = 0;
        pos += (Vector3)Random.insideUnitCircle*0.3f;

        instance.transform.position = pos;
        text.color = 
            info.isCrit?ColorLib.hitColor:
            info.isHeal?ColorLib.healColor:
            Color.white;
        text.alpha = 1;
        text.text = info.damage.ToString();
        text
            .DOFade(0, 0.2f)
            .SetDelay(0.3f);
        instance.transform
            .DOPunchScale(new(0.2f, 0.2f, 0), 0.1f);
        instance.transform
            .DOMoveY(instance.transform.position.y + 0.7f, 0.5f)
            .SetEase(Ease.OutExpo)
            .OnComplete(()=>pool.Release(instance));
    }
}
