using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private GameObjectPool headUpTextPool = new("HeadUpText");

    public static Player main;

    public bool godMode = false;
    private float baseMoveSpeed = 5;
    private int baseHealth = 100;

    //interaction
    private const float searchRadius = 10;
    private const float interactRadius = 2;
    private IInteractable _interactable;
    private IInteractable interactable
    {
        get { return _interactable; }
        set
        {
            if (_interactable == value) return;
            _interactable?.InspectEnd();
            _interactable = value;
            _interactable?.InspectStart();
        }
    }

    //components
    private PlayerInput input;
    private VisualHandler visual;
    public Pawn pawn;
    public Weapon weapon;
    public Hp hp;

    //input
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction aimAction;
    public Vector2 playerAimPosition;

    //states
    private bool canBeHit = true;
    private float immuneTimer = 0.2f;

    private bool isJumping = false;
    private bool isInAnimation = false;
    public bool isSleeping = false;

    [HideInInspector] public bool isFacingRight;
    [HideInInspector] public Vector2 playerAimDirection;

    //attack infos
    public AttackInfo attackInfo = new();
    public AttackInfo deathBlow;

    //shield & coin
    public static int shieldCount;
    private static int _coinCount;
    public static int coinCount
    {
        get { return _coinCount; }
        set
        {
            _coinCount = value;
            if (UIStats.main == null) return;
            UIStats.main.SetCoin(_coinCount);
        }
    }

    //perks
    public const int maxPerkCount = 35;
    public static int currentMaxPerkCount = 10;
    public static List<Perk> perks = new();

    //events
    public static Action onPerkChange;
    public static Action onPlayerSpawn;
    public static Action onEnemyKill;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Reset()
    {
        //reset some static variables
        perks = new();
        shieldCount = 0;
        coinCount = 0;
        onPerkChange = null;
        onPlayerSpawn = null;
        onEnemyKill = null;
        PerkUndying.used = false;
    }

    private void OnEnable()
    {
        //fetch components
        main = this;
        pawn = GetComponent<Pawn>();
        visual = GetComponent<VisualHandler>();
        hp = GetComponent<Hp>();
        input = GetComponent<PlayerInput>();

        pawn.moveSpeed = baseMoveSpeed * PlayerStats.moveSpeed;

        //assign hp delegate
        hp.maxHealth = baseHealth + PlayerStats.additionalHealth;
        hp.onReceiveAttack += OnReceiveAttack;
        hp.onDeath += OnDeath;

        //assign input action
        moveAction = input.actions.FindAction("Movement");
        attackAction = input.actions.FindAction("Attack");
        aimAction = input.actions.FindAction("Aim");

        //set hud ui
        UIManager.main.ShowHud();
        UIStats.SetTarget(hp);

        UpdateAttackInfo();
        onPlayerSpawn?.Invoke();
    }
    private void Update()
    {
        InputMovement();
        InputAttack();
        InputAim();
        FindInteractable();
    }

    public void UpdateAttackInfo()
    {
        attackInfo.attacker = Entity.Player;
        attackInfo.attackerName = Locale.Get("PLAYER");
        attackInfo.damage = (int)(10 * PlayerStats.damageMul);
        attackInfo.knockBack = 5 * PlayerStats.enemyKnockbackMul;
        attackInfo.bulletSpeed = 10;
        attackInfo.bulletMaxDist = 7 * PlayerStats.attackDistance;

        attackInfo.isStoned = HasPerk(Perk.PERK_STONED);
        attackInfo.stoneStrength = 1;

        if (HasPerk(Perk.PERK_HOMING))
        {
            attackInfo.isHoming = true;
            attackInfo.homingStrength = GetPerk(Perk.PERK_HOMING).level;
        }
        else attackInfo.isHoming = false;

        if (HasPerk(Perk.PERK_SALVO)) attackInfo.additionalBurst = GetPerk(Perk.PERK_SALVO).level;
        if (HasPerk(Perk.PERK_CHAINREACTION)) attackInfo.chainCount = GetPerk(Perk.PERK_CHAINREACTION).level;
        if (HasPerk(Perk.PERK_BOUNCE)) attackInfo.bounceCount = GetPerk(Perk.PERK_BOUNCE).level;
        attackInfo.penetrate = HasPerk(Perk.PERK_PENETRATION);

        if (HasPerk(Perk.PERK_FLAME)) attackInfo.attackEffect = AttackEffect.Fire;
        else attackInfo.attackEffect = AttackEffect.None;
    }

    #region Player Inputs
    public void InputJump(InputAction.CallbackContext context)
    {
        if (!HasPerk(Perk.PERK_JUMPER)) return;
        if (context.phase != InputActionPhase.Performed) return;
        if (isJumping) return;
        if (isInAnimation) return;
        if (isSleeping) return;

        isJumping = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore");
        visual.Jump(0.5f);
        this.Delay(0.5f, () =>
        {
            isJumping = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
        });
    }

    public void InputReload(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        if (weapon == null) return;
        weapon.StartReload();
    }

    public void InputInteract(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        if (isJumping) return;
        if (isInAnimation) return;
        if (isSleeping) return;
        if (interactable == null) return;

        interactable.Interact();
    }

    private void InputMovement()
    {
        if (isSleeping) return;
        if (isInAnimation) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude <= 0.1f) return;
        pawn.MoveInput(input.normalized);
    }

    private void InputAttack()
    {
        if (weapon == null) return;
        if (isJumping) return;
        if (isSleeping) return;
        if (isInAnimation) return;

        weapon.Attack(new()
        {
            isStarted = attackAction.WasPerformedThisFrame(),
            isPressing = attackAction.IsPressed(),
            isReleased = attackAction.WasReleasedThisFrame(),
            direction = playerAimDirection,
        });
    }

    private void InputAim()
    {
        if (isSleeping) return;
        //if (isInAnimation) return;

        Vector2 input = aimAction.ReadValue<Vector2>();
        if (input.magnitude > 0.1f) GameManager.main.isUsingController = true;

        if (GameManager.main.isUsingController)
        {
            if(input.magnitude > 0.5f)
            {
                Vector2 aimAssist = input.normalized;

                RaycastHit2D hit = Physics2D.CircleCast((Vector2)transform.position + input.normalized * 3, 2, playerAimDirection,attackInfo.bulletMaxDist, LayerMask.GetMask("Enemy"));
                if (hit)
                {
                    aimAssist = (hit.transform.position - transform.position + Vector3.up * 0.25f).normalized;
                    playerAimDirection = (playerAimDirection + (Vector2.Lerp(input.normalized, aimAssist, Settings.aimAssist*0.1f)-playerAimDirection)*Time.deltaTime*10).normalized;
                }
                else
                {
                    playerAimDirection = (playerAimDirection + (input.normalized - playerAimDirection) * Time.deltaTime * 40).normalized;
                }
            }
            playerAimPosition = (Vector2)transform.position + (playerAimDirection * 3);
        }
        else
        {
            playerAimPosition = CamController.main.mousePosition;
            playerAimDirection = (CamController.main.mousePosition - (Vector2)transform.position).normalized;
        }

        SetPlayerAim(); 
    }

    #endregion

    #region Perk
    private static void OnPerkChange()
    {
        if (main != null)
        {
            //update attack info
            main.UpdateAttackInfo();
            //update movespeed
            main.pawn.moveSpeed = main.baseMoveSpeed * PlayerStats.moveSpeed;
            //update weapon mag
            if(main.weapon != null) main.weapon.magSize = (int)(main.weapon.baseMagSize * PlayerStats.magSize);
            //update hp
            main.hp.maxHealth = main.baseHealth + PlayerStats.additionalHealth;
            UIStats.main.UpdateHp();
        }
        onPerkChange?.Invoke();
    }

    public static bool CanAddPerk(Perk perk)
    {
        if (perk == null) return false;

        Perk existing = GetPerk(perk.ID);
        if (existing == null) return perks.Count < currentMaxPerkCount;
        if (existing != null && existing.level == existing.maxLevel) return false;
        return true;
    }

    public static void AddPerk(Perk perk)
    {
        if (!CanAddPerk(perk)) return;
        Perk existing = GetPerk(perk.ID);
        //level up
        if (existing != null) { existing.level++; existing.OnLevelUp(); }
        //add new perk
        else { perks.Add(perk); perk.OnFirstActive(); }
        OnPerkChange();
    }

    public static void RemovePerk(Perk perk)
    {
        if (!perks.Contains(perk)) return;
        perk.OnDiscard();
        perks.Remove(perk);
        OnPerkChange();
    }

    public static void RemoveAndAddPerk(Perk oldPerk, Perk newPerk)
    {
        if (!perks.Contains(oldPerk)) return;
        oldPerk.OnDiscard();
        perks.Remove(oldPerk);
        AddPerk(newPerk);
    }

    public static bool HasPerk(string ID) => GetPerk(ID) != null;

    public static Perk GetPerk(string ID)
    {
        foreach (Perk perk in perks)
        {
            if (ID == perk.ID) return perk;
        }
        return null;
    }

    public static List<string> GetPerkIDList()
    {
        List<string> result = new();
        foreach (Perk perk in perks)
        {
            if (perk.level < perk.maxLevel) continue;
            result.Add(perk.ID);
        }
        return result;
    }
    #endregion

    #region Interaction
    private void FindInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, LayerMask.GetMask("Interactable"));
        Collider2D closest = null;
        float closestDist = searchRadius + 10;
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out IInteractable interact)) continue;
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (interact.IsInteractable() && dist < interactRadius && dist < closestDist)
            {
                closest = hit;
                closestDist = dist;
            }
        }
        if (closest == null) { interactable = null; return; }
        if (closest.TryGetComponent(out IInteractable target)) interactable = target;
    }
    #endregion

    #region Stats
    public void SetWeapon(string name)
    {
        if (name == "")
        {
            Destroy(weapon);
            weapon = null;
            return;
        }

        Weapon instance = Instantiate(WeaponLib.Get(name).prefab).GetComponent<Weapon>();
        instance.transform.SetParent(transform);
        weapon = instance;
        instance.magSize = (int)(instance.baseMagSize * PlayerStats.magSize);
    }

    public void AddShield()
    {
        shieldCount++;
        UIStats.main.SetShield(shieldCount);
    }

    private void UseShield()
    {
        shieldCount--;
        UIStats.main.SetShield(shieldCount);
    }

    public void AcquireCoin(int amount)
    {
        coinCount += amount;
        UIStats.main.SetCoin(coinCount);
    }
    #endregion

    #region HP
    private AttackInfo OnReceiveAttack(AttackInfo info)
    {
        if (!canBeHit) return new() { damage = 0 };
        if (isInAnimation) return new() { damage = 0 }; ;

        //use shield first
        if (shieldCount > 0)
        {
            info.damage = 0;
            UseShield();
            UIManager.main.HitVignetteShield();
        }
        else { UIManager.main.HitVignette(); }

        #region Perk Effects
        //tank
        if (HasPerk(Perk.PERK_TANK) && info.attackType == AttackType.Contact)
        {
            info.damage = Mathf.Max(1, info.damage - 2 * GetPerk(Perk.PERK_TANK).level);
        }

        //tenacious
        PerkTenacious perkTenacious = GetPerk(Perk.PERK_TENACIOUS) as PerkTenacious;
        if (hp.health > 1 && perkTenacious != null && !perkTenacious.isOnCooldown)
        {
            if(info.damage >= hp.health)
            {
                perkTenacious.isOnCooldown = true;
                this.Delay(perkTenacious.cooldown, () => perkTenacious.isOnCooldown = false);
                info.damage = hp.health - 1;
                info.attackEffect = AttackEffect.None;
            }
        }

        //undying
        if(!PerkUndying.used && HasPerk(Perk.PERK_UNDYING) && info.damage >= hp.health)
        {
            info.damage = 0;
            info.attackEffect = AttackEffect.None;
            hp.Heal(new() { damage = 20 - hp.health, isHeal = true });
            PerkUndying.used = true;
        }

        //flare perk
        if (HasPerk(Perk.PERK_FLARE))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2,LayerMask.GetMask("Enemy"));
            foreach (Collider2D hit in hits)
            {
                if(hit.TryGetComponent(out Pawn otherPawn))
                {
                    otherPawn.AddForce((hit.transform.position - transform.position).normalized * 10);
                }
            }
        }
        #endregion

        //misc effects
        canBeHit = false;
        this.Delay(immuneTimer, () => canBeHit = true);

        visual.sprite.HitEffect();
        pawn.AddForce(info.direction * 5 * info.knockBack);
        if (godMode) return new() { damage = 0 };
        return info;
    }

    private void OnDeath(AttackInfo info)
    {
        deathBlow = info;
        input.SwitchCurrentActionMap("UI");
        Instantiate(Prefab.Get("PlayerDeath"));
    }
    #endregion

    #region Traversal
    public void MoveRoom(Direction outDir, Vector2 outPos)
    {
        CamController.main.TranslateSystem(outPos - (Vector2)transform.position);
        transform.position = outPos;
        playerAimPosition = (Vector2)transform.position + (playerAimDirection * 3);
        Vector2 moveInput = outDir == Direction.Up ? new(0, 1) : outDir == Direction.Down ? new(0, -1) : outDir == Direction.Right ? new(1, 0) : new(-1, 0);
        StartCoroutine(MoveForSeconds(1, moveInput));
    }

    //used on changing room
    private IEnumerator MoveForSeconds(float second, Vector2 moveInput)
    {
        isInAnimation = true;
        float time = second;
        pawn.moveSpeed = baseMoveSpeed;
        while (time > 0)
        {
            time -= Time.deltaTime;
            pawn.MoveInput(moveInput);
            yield return null;
        }
        pawn.moveSpeed = baseMoveSpeed * PlayerStats.moveSpeed;
        isInAnimation = false;
    }
    #endregion

    #region Miscellaneous

    private void SetPlayerAim()
    {
        if (hp.isDead) return;
        float mouseDirectionX = playerAimDirection.x;
        if (mouseDirectionX > 0.5f) visual.sprite.flipX = true;
        else if (mouseDirectionX < -0.5f) visual.sprite.flipX = false;
        isFacingRight = visual.sprite.flipX;

        if (weapon != null) weapon.SetOrbitTransform(playerAimDirection);
    }

    public SpriteRenderer GetDeathSprite() => visual.sprite;

    public void ShowHeadUpText(string text)
    {
        TextMeshPro headUpText = headUpTextPool.Get().GetComponent<TextMeshPro>();
        headUpText.Delay(0.51f, () => headUpTextPool.Release(headUpText.gameObject));
        headUpText.transform.SetParent(transform);

        headUpText.text = text;
        headUpText.DOFade(1, 0.05f).SetEase(Ease.Linear);
        headUpText.DOFade(0, 0.45f).SetEase(Ease.Linear).SetDelay(0.05f);
        headUpText.transform.localPosition = Vector3.up * 1f;
        headUpText.transform.DOLocalMoveY(1.3f, 0.5f);
    }

    public void Sleep(bool state)
    {
        isSleeping = state;
    }
    #endregion
}
