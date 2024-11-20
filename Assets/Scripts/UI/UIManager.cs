using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    public static Transform mouseFollow;
    public static int screenWidth;
    public static int screenHeight;

    // info
    [HideInInspector] public float pixelsPerUnit;
    [HideInInspector] public float scaleFactor;
    private CanvasScaler scaler;

    [Header("Parameters")]
    public bool showHUD = true;
    public bool showHitVignette = true;
    public bool canPause = true;
    public bool isPaused = false;

    private bool overlayShown = false;
    private readonly List<Image> overlayTileList = new();
    private Tweener overlayTimeTweener;
    private Tweener overlayFadeTweener;

    [Header("References")]
    public Transform minimapParent;
    public Transform shopUIParent;
    public Transform perkParent;
    public GameObject gameOver;
    public RunResult runResult;
    public ShopPopup shopPopup;
    public Image reloadProgress;
    public PauseMenu pauseMenu;
    public PerkDiscardHandler perkDiscardHandler;
    public ShopHandler shopHandler;
    public AmmoCounter ammoCounter;
    public OverlayManager overlayManager;
    [SerializeField] private Transform overlayTileParent;
    [SerializeField] private Image fader;
    [SerializeField] private Image topFader;
    [SerializeField] private CanvasGroup overlayGroup;
    [SerializeField] private CanvasGroup hudGroup;
    [SerializeField] private Image hitVignette;
    [SerializeField] private Transform _mouseFollow;


    private void Awake()
    {
        if (main == null) main = this;
        else Destroy(gameObject);

        scaler = GetComponent<CanvasScaler>();
        pixelsPerUnit = scaler.referencePixelsPerUnit;
        scaleFactor = scaler.scaleFactor;
        mouseFollow = _mouseFollow;
        screenHeight = (int)scaler.referenceResolution.y;
        screenWidth = Mathf.CeilToInt(Screen.width * (screenHeight/(float)Screen.height));
        PauseMenu.main = pauseMenu;

        CreateOverlayBackgroundTile();
    }

    private void Update()
    {
        MouseFollow();
    }

    public void PauseAction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!canPause) return;
        if (!isPaused) { isPaused = true; PauseMenu.Open(); }
        else { isPaused = false; PauseMenu.Close(); }
    }

    public void TabAction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (overlayShown) HideOverlay();
        else ShowOverlay();
    }

    #region FadeInOut
    public void FadeInOut(float halfDuration) => fader.DOFade(1, halfDuration).SetUpdate(true).OnComplete(() => fader.DOFade(0, halfDuration).SetUpdate(true));
    public void FadeIn(float duration) => fader.DOFade(1, duration).SetUpdate(true);
    public void FadeOut(float duration) => fader.DOFade(0, duration).SetUpdate(true);
    public void TopFadeInOut(float halfDuration) => topFader.DOFade(1, halfDuration).OnComplete(() => topFader.DOFade(0, halfDuration).SetUpdate(true)).SetUpdate(true);
    public void TopFadeIn(float duration) => topFader.DOFade(1, duration).SetUpdate(true);
    public void TopFadeOut(float duration) => topFader.DOFade(0, duration).SetUpdate(true);
    #endregion

    #region Overlay

    public void ShowOverlay()
    {
        if (overlayShown) return;
        overlayShown = true;
        canPause = false;

        GameManager.PauseGame();
        ShowOverlayBackground();
        overlayFadeTweener = overlayGroup.DOFade(1, 0.2f).SetUpdate(true).SetDelay(0.1f);
        overlayGroup.blocksRaycasts = true;

        overlayManager.UpdatePerks();
    }

    public void HideOverlay()
    {
        if (!overlayShown) return;
        overlayShown = false;
        canPause = true;

        GameManager.ResumeGame();
        HideOverlayBackground();
        overlayFadeTweener = overlayGroup.DOFade(0, 0.2f).SetUpdate(true);
        overlayGroup.blocksRaycasts = false;
    }

    private void CreateOverlayBackgroundTile()
    {
        int xCount = Mathf.CeilToInt(screenWidth/100f);
        int yCount = Mathf.CeilToInt(screenHeight/100f);
        for (int y = 0; y < yCount + 1; y++)
        {
            for (int x = 0; x < xCount + 1; x++)
            {
                Transform instance = Instantiate(Prefab.Get("OverlayTile")).transform;
                instance.transform.SetParent(overlayTileParent);
                instance.localScale = Vector3.one;
                instance.localPosition = new(
                    (x * 100) + (Screen.width * 0.5f) % (100) - (50),
                    (y * 100) + (Screen.height * 0.5f) % (100) - (50), 0);
                overlayTileList.Add(instance.GetComponent<Image>());
            }
        }
    }

    public void ShowOverlayBackground()
    {
        foreach (Image image in overlayTileList)
        {
            float delay = Vector3.Distance(image.transform.position, new(Screen.width / (scaleFactor * 2), Screen.height / (scaleFactor * 2), 0)) * 0.0002f * 1920/Screen.width;
            image.DOFade(0.9f, 0.1f)
                .SetDelay(delay)
                .SetUpdate(true);
        }
    }

    public void HideOverlayBackground()
    {
        foreach (Image image in overlayTileList)
        {
            float delay = Vector3.Distance(image.transform.position, new(Screen.width / (scaleFactor * 2), Screen.height / (scaleFactor * 2), 0)) * 0.0002f * 1920 / Screen.width;
            image.DOFade(0, 0.1f)
                .SetDelay(delay)
                .SetUpdate(true);
        }
    }
    #endregion

    #region HUD

    public void ShowHud()
    {
        if (!showHUD) return;
        hudGroup.blocksRaycasts = false;
        hudGroup.DOFade(1, 0.6f).SetUpdate(true);
    }

    public void HideHud()
    {
        hudGroup.blocksRaycasts = false;
        hudGroup.DOFade(0, 0.6f).SetUpdate(true);
    }

    #endregion

    private void MouseFollow()
    {
        if (GameManager.main.isUsingController)
        {
            if (Player.main != null)
            {
                mouseFollow.transform.position = Camera.main.WorldToScreenPoint(Player.main.playerAimPosition);
            }
            else mouseFollow.transform.position = new(-100, -100);
        }
        else
        {
            mouseFollow.transform.position = Input.mousePosition * scaleFactor;
        }
    }

    public void HitVignette()
    {
        if (!showHitVignette) return;
        hitVignette.DOKill();
        hitVignette.color = ColorLib.hitColor.WithAlpha(0.5f);
        hitVignette.DOFade(0, 0.5f);
    }

    public void HitVignetteShield()
    {
        if (!showHitVignette) return;
        hitVignette.DOKill();
        hitVignette.color = ColorLib.shieldColor.WithAlpha(0.5f);
        hitVignette.DOFade(0, 0.5f);
    }

    public void SetReloadProgress(float progress)
    {
        reloadProgress.DOFade(1, 0.1f);
        //if (!reloadProgress.gameObject.activeSelf) reloadProgress.gameObject.SetActive(true);
        reloadProgress.fillAmount = progress;
    }
    public void HideReloadProgress()
    {
        reloadProgress.DOFade(0, 0.2f);
        //reloadProgress.gameObject.SetActive(false);
    }
}
