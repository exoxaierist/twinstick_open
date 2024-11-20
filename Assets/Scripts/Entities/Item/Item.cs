using UnityEngine;

[RequireComponent(typeof(VisualHandler))]
public class Item : MonoBehaviour, IInteractable
{
    public static void Spawn(string id, Vector3 position, Vector3 throwDir)
    {
        Spawn(Instantiate(ItemList.Get(id)).GetComponent<Item>(), position, throwDir);
    }
    public static void SpawnPerk(string perkId, Vector3 position, Vector3 throwDir)
    {
        PerkItem instance = Instantiate(ItemList.Get("ITEM_PERK")).GetComponent<PerkItem>();
        instance.Set(Perk.Get(perkId));
        Spawn(instance, position, throwDir);
    }
    public static void SpawnPerk(Perk perk, Vector3 position, Vector3 throwDir)
    {
        PerkItem instance = Instantiate(ItemList.Get("ITEM_PERK")).GetComponent<PerkItem>();
        instance.Set(perk);
        Spawn(instance, position, throwDir);
    }
    private static void Spawn(Item item, Vector3 position, Vector3 throwDir)
    {
        item.transform.position = position;
        item.canPickup = false;
        if (throwDir.sqrMagnitude > 0)
        {
            item.GetComponent<VisualHandler>().Jump(position + throwDir, throwDir.magnitude * 0.6f, throwDir.magnitude * 0.3f);
            Utility.GetMono().Delay(throwDir.magnitude * 0.7f, () =>
            {
                if (item != null) item.canPickup = true;
            });
        }
        else item.canPickup = true;
    }

    public bool canPickup = true;
    public ItemDisplayInfo info;
    public VisualHandler visual;
    public SpriteRenderer sprite;
    private DescriptionUI ui;

    private void Start()
    {
        info.name = Locale.Get(info.ID + "_NAME");
        info.description = Locale.Get(info.ID + "_DESC");
        visual = GetComponent<VisualHandler>();
        if (!info.ID.StartsWith("PERK")) visual.sprite.sprite = SpriteLib.Get(info.ID);
    }
    public bool CanInteract() => canPickup;
    public void Hide() { }
    public void Show() { }
    public void InspectStart() => ShowUI();
    public void InspectEnd() => HideUI();
    public void Interact() => OnInteract();

    protected virtual bool CanAcquire() => true;
    protected virtual void OnAcquire() { }

    /// <summary>
    /// override for deep rewriting pickup logic
    /// </summary>
    protected virtual void OnInteract()
    {
        if (CanAcquire()) Acquire();
    }

    protected void Acquire()
    {
        OnAcquire();
        Player.main.ShowHeadUpText(Locale.Get("MISC_YUM"));
        Effect.Play("Pop", EffectInfo.Pos(transform.position + Vector3.up * 0.1f));
        HideUI();
        Destroy(gameObject);
    }

    private void ShowUI()
    {
        if (ui != null) return;
        ui = DescriptionUI.GetWorldSpace(transform.position, info);
    }

    private void HideUI()
    {
        if (ui == null) return;
        DescriptionUI.Release(ui);
        ui = null;
    }
}
