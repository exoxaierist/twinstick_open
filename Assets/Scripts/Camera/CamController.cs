using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CamController : MonoBehaviour
{
    public static CamController main;

    public Transform followTarget;
    [Range(0,1)] public float lerpAlpha = 0.2f;
    [SerializeField] private float followSpeed = 2;
    [HideInInspector] public Vector2 mousePosition;

    public new static Camera camera;

    private Vector2 followPosition;
    private Vector2 shakeDelta;
    private float shakeRotDelta;

    private void Awake()
    {
        main = this;
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        GetMousePosition();
        FollowTarget();
        SetCameraTransform();
    }

    public void SetPosition(Vector2 pos)
    {
        followPosition = pos;
        camera.transform.SetPositionAndRotation(
            (Vector3)(followPosition + shakeDelta) + new Vector3(0, 0, -10),
            Quaternion.Euler(0, 0, shakeRotDelta));
    }

    public void Translate(Vector2 delta)
    {
        transform.Translate(delta);
        followPosition += delta;
    }

    public void Shake(float magnitude)
    {
        magnitude *= 1;
        DOTween
            .Shake(() => shakeDelta, x => shakeDelta = x, 0.7f*magnitude+0.4f, magnitude,20)
            .OnComplete(()=>shakeDelta = Vector2.zero);
        DOTween
            .Shake(() => new(0,shakeRotDelta), x => shakeRotDelta = x.y, 0.7f * magnitude + 0.4f, magnitude*20, 30)
            .OnComplete(() => DOTween.To(()=>shakeRotDelta,x=>shakeRotDelta=x,0,0.4f));
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(magnitude * 10, magnitude * 5);
            this.Delay(magnitude, () => Gamepad.current.SetMotorSpeeds(0, 0));
        }
    }

    private void SetCameraTransform()
    {
        camera.transform.SetPositionAndRotation(
            (Vector3)(followPosition + shakeDelta) + new Vector3(0,0,-10), 
            Quaternion.Euler(0, 0, shakeRotDelta));
    }

    private void FollowTarget()
    {
        if (followTarget == null) return;
        Vector2 targetPosition = Vector2.zero;
        if (GameManager.main.isUsingController)
        {
            if(Player.main!=null)
                targetPosition = Vector2.Lerp(followTarget.position, Player.main.playerAimPosition, 0.7f);
            followPosition += Mathf.Min(1,followSpeed * Time.unscaledDeltaTime * 0.5f) * (targetPosition - followPosition);
        }
        else
        {
            targetPosition = Vector2.Lerp(followTarget.position, mousePosition, lerpAlpha);
            followPosition += Mathf.Min(1,followSpeed * Time.unscaledDeltaTime) * (targetPosition - followPosition);
        }
    }

    private void GetMousePosition()
    {
        if (Player.main != null && Player.main.isSleeping) return;
        mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
