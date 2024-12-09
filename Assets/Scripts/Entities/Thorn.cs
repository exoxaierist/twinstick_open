using UnityEngine;

public class Thorn : MonoBehaviour
{
    public const int stepCount = 8;
    public static int currentStep;

    [Range(1, stepCount)] public int startingStep = 1;
    [Range(1, stepCount)] public int endingStep = 4; //exclusive

    public Sprite retracted;
    public Sprite ready;
    public Sprite thornOut;

    private int readyStep;
    private SpriteRenderer sprite;
    private bool isThornOut = false;
    private static bool canDamage = true;

    private AttackInfo info = new()
    {
        damage = 10, attacker = Entity.Gimic, attackType = AttackType.Contact, 
    };

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        readyStep = ((startingStep - 2 + stepCount) % stepCount)+1;
        ThornCounter.onCountChange += OnChange;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(isThornOut && canDamage && col.gameObject == Player.main.gameObject)
        {
            Player.main.hp.Damage(info);
            canDamage = false;
            Utility.GetMono().Delay(1, () => canDamage = true);
        }
    }

    private void OnChange()
    {
        if (sprite == null) return;
        if (currentStep == startingStep) { sprite.sprite = thornOut; isThornOut = true; }
        else if (currentStep == endingStep) { sprite.sprite = retracted; isThornOut = false; }
        else if (currentStep == readyStep) { sprite.sprite = ready; isThornOut = false; }
    }
}
