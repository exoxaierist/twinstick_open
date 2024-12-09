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
    public const string PERK_GOLDDIGGER = "PERK_GOLDDIGGER";
    public const string PERK_JUMPER = "PERK_JUMPER";
    public const string PERK_BULLETTIME = "PERK_BULLETTIME";
    public const string PERK_FLARE = "PERK_FLARE";
    public const string PERK_DAMAGE = "PERK_DAMAGE";
    public const string PERK_METABOLISM = "PERK_METABOLISM";
    public const string PERK_AUTORELOAD = "PERK_AUTORELOAD";
    public const string PERK_PROCRASTINATION = "PERK_PROCRASTINATION";
    public const string PERK_SHIELD = "PERK_SHIELD";
    public const string PERK_UNDYING = "PERK_UNDYING";
    public const string PERK_DANGERZONE = "PERK_DANGERZONE";
    public const string PERK_UNBEATEN = "PERK_UNBEATEN";

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
    public const string PERK_HEAVIERCHOICE = "PERK_HEAVIERCHOICE";
    public const string PERK_PENETRATION = "PERK_PENETRATION";
    public const string PERK_LONGREACH = "PERK_LONGREACH";
    public const string PERK_FASTHANDS = "PERK_FASTHANDS";
    public const string PERK_SLUG = "PERK_SLUG";
    public const string PERK_FLAME = "PERK_FLAME";
    public const string PERK_EXPLOSION = "PERK_EXPLOSION";
    public const string PERK_RADIAL = "PERK_RADIAL";
    public const string PERK_AGING = "PERK_AGING";
    public const string PERK_XL = "PERK_XL";


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
        PERK_GOLDDIGGER,
        PERK_JUMPER,
        PERK_BULLETTIME,
        PERK_FLARE,
        PERK_DAMAGE,
        PERK_METABOLISM,
        PERK_AUTORELOAD,
        PERK_PROCRASTINATION,
        PERK_SHIELD,
        PERK_UNDYING,
        PERK_DANGERZONE,
        PERK_UNBEATEN,

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
        PERK_HEAVIERCHOICE,
        PERK_PENETRATION,
        PERK_LONGREACH,
        PERK_FASTHANDS,
        PERK_SLUG,
        PERK_FLAME,
        PERK_EXPLOSION,
        PERK_RADIAL,
        PERK_AGING,
        PERK_XL,
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

        return allCopy[Random.Range(0, allCopy.Count)];
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
            case PERK_GOLDDIGGER: return new PerkGoldDigger();
            case PERK_JUMPER: return new PerkJumper();
            case PERK_BULLETTIME: return new PerkBulletTime();
            case PERK_FLARE: return new PerkFlare();
            case PERK_DAMAGE: return new PerkDamage();
            case PERK_METABOLISM: return new PerkMetabolism();
            case PERK_AUTORELOAD: return new PerkAutoReload();
            case PERK_PROCRASTINATION: return new PerkProcrastination();
            case PERK_SHIELD: return new PerkShield();
            case PERK_UNDYING: return new PerkUndying();
            case PERK_DANGERZONE: return new PerkDangerzone();
            case PERK_UNBEATEN: return new PerkUnbeaten();

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
            case PERK_HEAVIERCHOICE:return new PerkHeavierChoice();
            case PERK_PENETRATION:return new PerkPenetration();
            case PERK_LONGREACH:return new PerkLongReach();
            case PERK_FASTHANDS:return new PerkFastHands();
            case PERK_SLUG: return new PerkSlug();
            case PERK_FLAME: return new PerkFlame();
            case PERK_EXPLOSION: return new PerkExplosion();
            case PERK_RADIAL: return new PerkRadial();
            case PERK_AGING: return new PerkAging();
            case PERK_XL: return new PerkXL();
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
