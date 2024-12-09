using DG.Tweening;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    public static SoundSystem main;
    public const string UI_HOVER = "UI_HOVER";
    public const string UI_SUBMIT = "UI_SUBMIT";

    private const string PLAYER_WALK_01 = "PLAYER_WALK_01";
    private const string PLAYER_WALK_02 = "PLAYER_WALK_02";
    private const string PLAYER_WALK_03 = "PLAYER_WALK_03";
    private const string PLAYER_WALK_04 = "PLAYER_WALK_04";
    private const string PLAYER_WALK_05 = "PLAYER_WALK_05";
    private const string PLAYER_WALK_06 = "PLAYER_WALK_06";
    private const string PLAYER_WALK_07 = "PLAYER_WALK_07";
    private const string PLAYER_WALK_08 = "PLAYER_WALK_08";
    public const string PLAYER_RELOAD_START = "PLAYER_RELOAD_START";
    public const string PLAYER_RELOAD_END = "PLAYER_RELOAD_END";

    public const string ACTION_SHOOT_01 = "ACTION_SHOOT_01";
    public const string ACTION_SHOOT_02 = "ACTION_SHOOT_02";
    public const string ACTION_SHOOT_03 = "ACTION_SHOOT_03";
    public const string ACTION_SHOOT_04 = "ACTION_SHOOT_04";
    public const string ACTION_SHOOT_05 = "ACTION_SHOOT_05";
    public const string ACTION_SHOOT_06 = "ACTION_SHOOT_06";
    private const string ACTION_SHOOT_ENEMY_01 = "ACTION_SHOOT_ENEMY_01";

    private const string ACTION_HIT_01 = "ACTION_HIT_01";
    private const string ACTION_HIT_02 = "ACTION_HIT_02";
    private const string ACTION_HIT_03 = "ACTION_HIT_03";
    private const string ACTION_HIT_04 = "ACTION_HIT_04";
    private const string ACTION_HIT_05 = "ACTION_HIT_05";
    private const string ACTION_HIT_06 = "ACTION_HIT_06";
    private const string ACTION_HIT_07 = "ACTION_HIT_07";
    private const string ACTION_HIT_08 = "ACTION_HIT_08";
    private const string ACTION_HIT_09 = "ACTION_HIT_09";

    private const string ACTION_HIT_SECONDARY_01 = "ACTION_HIT_SECONDARY_01";
    private const string ACTION_HIT_SECONDARY_02 = "ACTION_HIT_SECONDARY_02";
    private const string ACTION_HIT_SECONDARY_03 = "ACTION_HIT_SECONDARY_03";
    private const string ACTION_HIT_SECONDARY_04 = "ACTION_HIT_SECONDARY_04";
    private const string ACTION_HIT_SECONDARY_05 = "ACTION_HIT_SECONDARY_05";

    public const string ACTION_JUMP = "ACTION_JUMP";
    private const string ACTION_EXPLOSION_01 = "ACTION_EXPLOSION_01";
    private const string ACTION_EXPLOSION_02 = "ACTION_EXPLOSION_02";
    private const string ACTION_EXPLOSION_03 = "ACTION_EXPLOSION_03";
    private const string ACTION_EXPLOSION_04 = "ACTION_EXPLOSION_04";
    private const string ACTION_EXPLOSION_05 = "ACTION_EXPLOSION_05";

    public const string ENEMY_SPAWN = "ENEMY_SPAWN";
    public const string ENEMY_DEATH_01 = "ENEMY_DEATH_01";
    public const string ENEMY_CRACKLE = "ENEMY_CRACKLE";

    public const string COIN_SPAWN = "COIN_SPAWN";
    public const string COIN_ACQUIRE = "COIN_ACQUIRE";

    public const string MISC_DOOR_OPEN = "MISC_DOOR_OPEN";
    public const string MISC_DOOR_CLOSE = "MISC_DOOR_CLOSE";
    public const string MISC_GULP = "MISC_GULP";
    public const string MISC_KACHING = "MISC_KACHING";

    private const string MISC_SHATTER_01 = "MISC_SHATTER_01";
    private const string MISC_SHATTER_02 = "MISC_SHATTER_02";
    private const string MISC_SHATTER_03 = "MISC_SHATTER_03";

    public const string MISC_ERROR = "MISC_ERROR";
    public const string MISC_DEATH = "MISC_DEATH";

    public static string[] all = new string[]
    {
        UI_HOVER,
        UI_SUBMIT,

        PLAYER_WALK_01,
        PLAYER_WALK_02,
        PLAYER_WALK_03,
        PLAYER_WALK_04,
        PLAYER_WALK_05,
        PLAYER_WALK_06,
        PLAYER_WALK_07,
        PLAYER_WALK_08,
        PLAYER_RELOAD_START,
        PLAYER_RELOAD_END,

        ACTION_SHOOT_01,
        ACTION_SHOOT_02,
        ACTION_SHOOT_03,
        ACTION_SHOOT_04,
        ACTION_SHOOT_05,
        ACTION_SHOOT_06,
        ACTION_SHOOT_ENEMY_01,

        ACTION_HIT_01,
        ACTION_HIT_02,
        ACTION_HIT_03,
        ACTION_HIT_04,
        ACTION_HIT_05,
        ACTION_HIT_06,
        ACTION_HIT_07,
        ACTION_HIT_08,
        ACTION_HIT_09,
        ACTION_HIT_SECONDARY_01,
        ACTION_HIT_SECONDARY_02,
        ACTION_HIT_SECONDARY_03,
        ACTION_HIT_SECONDARY_04,
        ACTION_HIT_SECONDARY_05,

        ACTION_JUMP,
        ACTION_EXPLOSION_01,
        ACTION_EXPLOSION_02,
        ACTION_EXPLOSION_03,
        ACTION_EXPLOSION_04,
        ACTION_EXPLOSION_05,

        ENEMY_SPAWN,
        ENEMY_DEATH_01,
        ENEMY_CRACKLE,

        COIN_SPAWN,
        COIN_ACQUIRE,

        MISC_DOOR_OPEN,
        MISC_DOOR_CLOSE,
        MISC_GULP,
        MISC_KACHING,
        MISC_SHATTER_01,
        MISC_SHATTER_02,
        MISC_SHATTER_03,
        MISC_ERROR,
        MISC_DEATH,
    };
    public static string[] ACTION_SHOOT = new string[] { ACTION_SHOOT_01, ACTION_SHOOT_02, ACTION_SHOOT_03, ACTION_SHOOT_04, ACTION_SHOOT_05, ACTION_SHOOT_06 };
    public static string[] ACTION_HIT_SECONDARY = new string[] {ACTION_HIT_SECONDARY_01,ACTION_HIT_SECONDARY_02,ACTION_HIT_SECONDARY_03,ACTION_HIT_SECONDARY_04,ACTION_HIT_SECONDARY_05 };
    public static string[] PLAYER_WALK = new string[] { PLAYER_WALK_01, PLAYER_WALK_02, PLAYER_WALK_03, PLAYER_WALK_04, PLAYER_WALK_05, PLAYER_WALK_06, PLAYER_WALK_07, PLAYER_WALK_08 };
    public static string[] ACTION_HIT = new string[] { ACTION_HIT_01, ACTION_HIT_02, ACTION_HIT_03, ACTION_HIT_04, ACTION_HIT_05,ACTION_HIT_06,ACTION_HIT_07,ACTION_HIT_08,ACTION_HIT_09 };
    public static string[] ACTION_SHOOT_ENEMY = new string[] { ACTION_SHOOT_ENEMY_01 };
    public static string[] ACTION_EXPLOSION = new string[] { ACTION_EXPLOSION_01, ACTION_EXPLOSION_02, ACTION_EXPLOSION_03, ACTION_EXPLOSION_04, ACTION_EXPLOSION_05 };
    public static string[] MISC_SHATTER = new string[] { MISC_SHATTER_01, MISC_SHATTER_02, MISC_SHATTER_03 };
    private static GameObjectPool pool = new("SoundPlayer");

    public static void Play(string key)
    {
        if (!Application.isPlaying) return;
        main.source.PlayOneShot(SoundResource.Get(key),Settings.sfxV * Settings.masterV * 0.01f);
    }

    public static void Play(string key, Vector3 position, float volume = 1)
    {
        AudioSource instance = pool.Get().GetComponent<AudioSource>();

        position.z = -10;
        instance.transform.position = position;
        instance.spatialBlend = 0.7f;
        instance.volume = volume * Settings.sfxV * Settings.masterV * 0.01f;
        //instance.pitch = Random.Range(0.95f,1.05f);

        instance.PlayOneShot(SoundResource.Get(key));
        main.DelayRealtime(SoundResource.Get(key).length, () => pool.Release(instance.gameObject));
    }

    public static void SetVolume()
    {
        if (main == null) return;
        main.musicPlayer.DOKill();
        main.source.volume = Settings.sfxV * Settings.masterV * 0.01f;
        main.musicPlayer.volume = Settings.musicV * Settings.masterV * 0.005f;
    }

    public static void FadeMusic()
    {
        if (main == null) return;
        main.musicPlayer.DOKill();
        main.musicPlayer.DOFade(0.15f * Settings.musicV * Settings.masterV * 0.005f, 0.5f).SetUpdate(true);
    }

    public static void UnfadeMusic()
    {
        if (main == null) return;
        main.musicPlayer.DOKill();
        main.musicPlayer.DOFade(Settings.musicV * Settings.masterV * 0.005f, 0.5f).SetUpdate(true);
    }

    private AudioSource source;
    public AudioSource musicPlayer;
    public AudioClip[] musicList;
    private int lastIndex = -1;

    private void Awake()
    {
        if (main != null) Destroy(gameObject);
        else main = this;
        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayNextMusic();
        musicPlayer.volume = 0;
        musicPlayer.DOFade(Settings.musicV * Settings.masterV * 0.005f, 1).SetUpdate(true);
    }

    private void PlayNextMusic()
    {
        if (musicPlayer == null) return;
        int random = lastIndex;
        while(random == lastIndex)
        {
            random = Random.Range(0, musicList.Length);
        }
        lastIndex = random;
        musicPlayer.clip = musicList[random];
        musicPlayer.Play();
        this.DelayRealtime(musicPlayer.clip.length, PlayNextMusic);
    }
}
