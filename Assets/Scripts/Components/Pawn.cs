using UnityEngine;

public class Pawn : MonoBehaviour
{
    public static GameObjectPool walkParticlePool = new("EnemyWalkParticle");

    [Header("Effects")]
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
    public LayerMask mask;
    public float radius = 0.5f;
    private readonly float skinWidth = 0.05f;

    [Header("Debug")]
    [SerializeField] private bool showRadius;

    //internal
    private Vector2 inputDir;
    public Vector2 unscaledVelocity { get; private set; }

    private void Start()
    {
        showWalkMotion = TryGetComponent(out visual);
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

    private void Move()
    {
        if (inputDir.sqrMagnitude == 0 && unscaledVelocity.sqrMagnitude == 0) return;
        if (Time.timeScale <= 0.0001f || Time.deltaTime == 0) return;

        Vector2 frameMovement = unscaledVelocity;
        frameMovement = Vector2.MoveTowards(frameMovement, inputDir * moveSpeed, accelRate * Time.deltaTime) * Time.deltaTime;

        Vector2 traceOrigin = transform.position;
        Vector2 traceDir = frameMovement.normalized;
        Vector2 targetPosition = traceOrigin + frameMovement;
        float traceDistance = frameMovement.magnitude;
        float penetrateSkinDepth = 0;

        for (int i = 0; i < 5; i++)
        {
            if (traceDistance == 0) break;

            RaycastHit2D hit = Physics2D.CircleCast(traceOrigin, radius, traceDir, traceDistance + 1, mask.value);

            //corner stuck fix
            if (hit && Vector2.Dot(hit.normal, traceDir) > -0.0001f)
            {
                traceOrigin += hit.normal * 0.001f;
                continue;
            }
            //calc skin depth
            if (hit) penetrateSkinDepth = (skinWidth / -Vector2.Dot(hit.normal, traceDir));
            if (!hit || (hit && hit.distance - penetrateSkinDepth > traceDistance))
            {
                //didn't hit anything
                targetPosition = traceOrigin + traceDir * traceDistance;
                break;
            }

            //move trace origin to contact point
            traceOrigin += traceDir * Mathf.Max(0, hit.distance - penetrateSkinDepth);
            traceDistance -= Mathf.Max(0, hit.distance - penetrateSkinDepth);
            targetPosition = traceOrigin;

            //calc normal force
            //align surface tangent to traceDir
            Vector2 tangent = hit.normal.RightOrtho();
            tangent *= Mathf.Sign(Vector2.Dot(tangent, traceDir));
            Vector2 oldTraceDir = traceDir;
            float oldTraceDistance = traceDistance;
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
        //restore unscaled velocity
        unscaledVelocity = frameMovement / Time.deltaTime;
        inputDir = Vector2.zero;
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

    private void OnDrawGizmos()
    {
        if (!showRadius) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, radius + skinWidth);
    }
}
