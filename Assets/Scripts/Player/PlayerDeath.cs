using DG.Tweening;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Transform square;
    private SpriteRenderer original;
    private SpriteRenderer copy;
    private SpriteRenderer background;

    private void OnEnable()
    {
        transform.position = Player.main.transform.position;
        UIManager.main.HideHud();
        UIManager.main.HideOverlay();
        UIManager.main.gameOver.SetActive(true);
        UIManager.main.canPause = false;
        UIManager.main.showHitVignette = false;

        original = Player.main.GetDeathSprite();
        copy = Instantiate(original.gameObject).GetComponent<SpriteRenderer>();
        background = GetComponent<SpriteRenderer>();
        Color red = background.color;
        copy.sortingLayerName = "Top";
        background.sortingLayerName = "Top";
        copy.sortingOrder = 10;
        background.sortingOrder = 9;
        copy.color = Color.black;
        square.transform.DOScale(Vector3.one*0.5f, 5f).SetUpdate(true);

        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.01f, 0.8f).SetUpdate(true);
        DOTween.To(() => CamController.main.lerpAlpha, x => CamController.main.lerpAlpha= x, 0, 3).SetUpdate(true);
        DOTween.To(() => CamController.camera.orthographicSize, x => CamController.camera.orthographicSize= x, 7, 3).SetUpdate(true).SetEase(Ease.OutCubic);
        copy.transform.DORotate(new(0, 0, 8), 2.3f).SetUpdate(true);
        this.DelayRealtime(0.3f, () => UIManager.main.TopFadeIn(2f));
        this.DelayRealtime(2.5f, () => OnFinish());
        SyncTransform();
    }

    private void Update()
    {
        SyncTransform();
    }

    private void SyncTransform()
    {
        if (copy == null) return;
        copy.transform.position = original.transform.position;
        square.transform.position = original.transform.position + Vector3.up*0.5f;
    }

    private void MakeRunInfo()
    {

    }

    private void OnFinish()
    {
        gameObject.SetActive(false);
        copy.transform.DOKill();
        Destroy(copy.gameObject);
        Time.timeScale = 1;
        UIManager.main.gameOver.SetActive(false);
        UIManager.main.FadeIn(0.01f);
        UIManager.main.TopFadeOut(0.5f);
        UIManager.main.runResult.ShowResult();
    }
}
