public class PerkUnbeaten : Perk
{
    public PerkUnbeaten()
    {
        ID = PERK_UNBEATEN;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }

    private bool isCombatRoom = false;
    private bool addDamage = true;

    public override void OnFirstActive()
    {
        if (Player.main == null) Player.onPlayerSpawn += OnActivation;
        else OnActivation();
    }

    public override void OnDiscard()
    {
        LevelManager.main.onRoomChange -= OnRoomChange;
        Player.onPlayerSpawn -= OnActivation;
        if(Player.main != null) Player.main.hp.onDamage -= OnDamage;
    }

    private void OnActivation()
    {
        Player.onPlayerSpawn -= OnActivation;
        Player.main.hp.onDamage += OnDamage;
        LevelManager.main.onRoomChange += OnRoomChange;
    }

    private void OnRoomChange(MetaRoomInfo to)
    {
        if (isCombatRoom && addDamage) PlayerStats.damageMul += 0.05f;
        if (!to.metaRoom.isFriendly) isCombatRoom = true;
        addDamage = true;
    }

    private void OnDamage()
    {
        addDamage = false;
    }
}
