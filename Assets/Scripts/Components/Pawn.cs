using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public static GameObjectPool walkParticlePool = new("EnemyWalkParticle");

    [Header("Motion")]
    public bool showWalkMotion = false;
    public bool showParticle = false;
    private bool isCustomParticle = false;

    private VisualHandler visual;
    public ParticleSystem particle;

    [Header("Movement")]
    public float moveSpeed = 3;
    public float accelRate = 30f;
    public float bounce = 0;
    public bool isJumping = false;

    [Header("Collision")]
    [SerializeField] private LayerMask mask;
    public float radius = 0.5f;
    private readonly float skinWidth = 0.05f;

    [Header("Debug")]
    [SerializeField] private bool showRadius;

    //internal
    private Vector2 inputDir;
    public Vector2 unscaledVelocity { get; private set; }
    private Coroutine moveRoutine;

    //callbacks
    public Action<Vector2> onHitWall;

    private void Start()
    {
        if (TryGetComponent(out visual)) showWalkMotion = true;
        if (particle != null) { showParticle = true; isCustomParticle = true; }
        else if (showParticle)
        {
            particle = walkParticlePool.Get().GetComponent<ParticleSystem>();
            particle.Stop();
        }
    }

    private void Update()
    {
        Move();
        if (showParticle)
        {
            particle.transform.position = transform.position;
            if (particle.isStopped) particle.Play();
        }
    }

    private void OnDestroy()
    {
        if(particle!=null && !isCustomParticle) walkParticlePool.Release(particle.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!showRadius) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, radius+skinWidth);
    }

    public void MoveForSeconds(Vector2 direction, float duration)
    {
        if (moveRoutine != null) return;
        moveRoutine = StartCoroutine(MoveRoutine(direction, duration));
    }
    public void StopMoveForSeconds()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = null;
    }
    private IEnumerator MoveRoutine(Vector2 direction, float duration)
    {
        while(duration > 0)
        {
            MoveInput(direction);
            duration -= Time.deltaTime;
            yield return null;
        }
        moveRoutine = null;
    }

    public void Jump(Vector2 delta, float duration)
    {
        if (isJumping) return;
        isJumping = true;
        this.Delay(duration, () => isJumping = false);

        visual.Jump((Vector2)transform.position + delta,duration*3,duration);
    }

    public void MoveInput(Vector2 _inputDir)
    {
        inputDir = _inputDir.normalized;
        if (showWalkMotion) visual.WalkMotion();
    }

    public void AddForce(Vector2 force)
    {
        unscaledVelocity += force;
    }

    public Vector2 GetDirToMove()
    {
        Vector2 result = Vector2.zero;
        for (int i = 0; i < 10; i++)
        {
            result = UnityEngine.Random.insideUnitCircle.normalized;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, result, 5, mask.value);
            if (hit && hit.distance > 1) return result;
        }
        return result;
    }

    private void Move()
    {
        if (inputDir.sqrMagnitude == 0 && unscaledVelocity.sqrMagnitude == 0) return;
        if (Time.timeScale <= 0.0001f || Time.deltaTime==0) return;

        Vector2 frameMovement = unscaledVelocity;
        frameMovement = Vector2.MoveTowards(frameMovement, inputDir * moveSpeed, accelRate*Time.deltaTime) * Time.deltaTime;

        Vector2 traceOrigin = transform.position;
        Vector2 traceDir = frameMovement.normalized;
        Vector2 targetPosition = traceOrigin + frameMovement;
        float traceDistance = frameMovement.magnitude;
        float penetrateSkinWidth = 0;

        for (int i = 0; i < 5; i++)
        {
            if (traceDistance == 0) break;

            RaycastHit2D hit = Physics2D.CircleCast(traceOrigin, radius, traceDir, traceDistance + 1,mask.value);
            
            //corner stuck fix
            if (hit && Vector2.Dot(hit.normal, traceDir) > -0.0001f)
            {
                traceOrigin += hit.normal * 0.001f;
                continue;
            }
            if (hit) penetrateSkinWidth = (skinWidth / -Vector2.Dot(hit.normal, traceDir));
            if (!hit || (hit && hit.distance - penetrateSkinWidth > traceDistance))
            {
                targetPosition = traceOrigin + traceDir * traceDistance;
                break;
            }

            traceOrigin += traceDir * Mathf.Max(0, hit.distance - penetrateSkinWidth);
            traceDistance -= Mathf.Max(0, hit.distance - penetrateSkinWidth);
            targetPosition = traceOrigin;

            //calc normal force
            if (Vector2.Dot(hit.normal, traceDir) < 0) onHitWall?.Invoke(hit.normal);
            Vector2 tangent = new(hit.normal.y, -hit.normal.x);
            Vector2 oldTraceDir = traceDir;
            float oldTraceDistance = traceDistance;
            tangent *= Mathf.Sign(Vector2.Dot(tangent, oldTraceDir));
            traceDir = tangent;
            traceDistance *= Vector2.Dot(tangent, oldTraceDir);

            //calc bounce
            Vector2 reflect = Vector2.Reflect(oldTraceDir, hit.normal);
            traceDir = Vector3.Slerp(traceDir, reflect, bounce);
            traceDistance = Mathf.LerpUnclamped(traceDistance, oldTraceDistance, bounce);
            traceDir.Normalize();
        }

        frameMovement = targetPosition - (Vector2)transform.position;
        transform.Translate(frameMovement);
        unscaledVelocity = frameMovement / Time.deltaTime;
        inputDir = Vector2.zero;
    }
}
