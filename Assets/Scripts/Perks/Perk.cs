using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Perk
{
    public Sprite sprite;
    public string ID;
    public string name;
    public string description;
    public int level;
    public int maxLevel;

    public const string PERK_MORNINGCOFFEE = "PERK_MORNINGCOFFEE";
    public const string PERK_EYEFOREYE = "PERK_EYEFOREYE";
    public const string PERK_HOMEOSTASIS = "PERK_HOMEOSTASIS";
    public const string PERK_FATADRENALINE = "PERK_FATADRENALINE";
    public const string PERK_HEATTREATED = "PERK_HEATTREATED";
    public const string PERK_TENACIOUS = "PERK_TENACIOUS";
    public const string PERK_TANK = "PERK_TANK";
    public const string PERK_SPIKE = "PERK_SPIKE";
    public const string PERK_REGEN = "PERK_REGEN";

    public const string PERK_BLINDSHOOTER = "PERK_BLINDSHOOTER";
    public const string PERK_SALVO = "PERK_SALVO";
    public const string PERK_CRITS = "PERK_CRITS";
    public const string PERK_HARDHIT = "PERK_HARDHIT";
    public const string PERK_STONED = "PERK_STONED";
    public const string PERK_CHAINREACTION = "PERK_CHAINREACTION";
    public const string PERK_HOMING = "PERK_HOMING";
    public const string PERK_COVERSIX = "PERK_COVERSIX";
    public const string PERK_BOUNCE = "PERK_BOUNCE";
    public const string PERK_RAPIDFIRE = "PERK_RAPIDFIRE";
    public const string PERK_BIGGERWAND = "PERK_BIGGERWAND";
    public const string PERK_LIGHTERCHOICE = "PERK_LIGHTERCHOICE";

    public static readonly string[] all =
    new string[]{
        PERK_MORNINGCOFFEE,
        PERK_EYEFOREYE,
        PERK_HOMEOSTASIS,
        PERK_FATADRENALINE,
        PERK_HEATTREATED,
        PERK_TENACIOUS,
        PERK_TANK,
        PERK_SPIKE,
        PERK_REGEN,

        PERK_BLINDSHOOTER,
        PERK_SALVO,
        PERK_CRITS,
        PERK_HARDHIT,
        PERK_STONED,
        PERK_CHAINREACTION,
        PERK_HOMING,
        PERK_COVERSIX,
        PERK_BOUNCE,
        PERK_RAPIDFIRE,
        PERK_BIGGERWAND,
        PERK_LIGHTERCHOICE,
    };

    public virtual void OnFirstActive() { }
    public virtual void OnDiscard() { }
    public virtual void OnLevelUp() { }

    public static string GetRandomID(List<string> inv)
    {
        List<string> allCopy = all.ToList();
        for (int i = 0; i < inv.Count; i++)
        {
            for (int j = 0; j < allCopy.Count; j++)
            {
                if (inv[i] == allCopy[j])
                {
                    allCopy.RemoveAt(j);
                    break;
                }
            }
        }

        return allCopy[Random.Range(0, allCopy.Count - 1)];
    }

    public static Perk Get(string perkID)
    {
        switch (perkID)
        {
            case PERK_MORNINGCOFFEE: return new PerkMorningCoffee();
            case PERK_EYEFOREYE: return new PerkEyeForEye();
            case PERK_HOMEOSTASIS: return new PerkHomeostasis();
            case PERK_FATADRENALINE: return new PerkFatAdrenaline();
            case PERK_HEATTREATED: return new PerkHeatTreated();
            case PERK_TENACIOUS: return new PerkTenacious();
            case PERK_TANK: return new PerkTank();
            case PERK_SPIKE: return new PerkSpike();
            case PERK_REGEN: return new PerkRegen();

            case PERK_BLINDSHOOTER: return new PerkBlindShooter();
            case PERK_SALVO: return new PerkSalvo();
            case PERK_CRITS: return new PerkCrits();
            case PERK_HARDHIT: return new PerkHardHit();
            case PERK_STONED: return new PerkStoned();
            case PERK_CHAINREACTION: return new PerkChainReaction();
            case PERK_HOMING: return new PerkHoming();
            case PERK_COVERSIX: return new PerkCoverSix();
            case PERK_BOUNCE: return new PerkBounce();
            case PERK_RAPIDFIRE:return new PerkRapidFire();
            case PERK_BIGGERWAND:return new PerkBiggerWand();
            case PERK_LIGHTERCHOICE:return new PerkLighterChoice();
        }
        Debug.LogError("perk id was not found: " + perkID);
        return null;
    }

    protected void OnInstantiate()
    {
        name = Locale.Get(ID + "_NAME");
        description = Locale.Get(ID + "_DESC");
        sprite = SpriteLib.Get(ID);
    }

    public override string ToString() => name;
}
