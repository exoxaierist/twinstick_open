using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static GameObjectPool pool = new("Bomb");
    public static void Throw(Vector2 startPos, Vector2 targetPos, AttackInfo info)
    {
        Bomb instance = pool.Get().GetComponent<Bomb>();
        instance.transform.position = startPos;
        instance.InternalThrow(targetPos, info);
    }

    private VisualHandler visual;

    private void InternalThrow(Vector2 targetPos, AttackInfo info)
    {
        visual = GetComponent<VisualHandler>();
        visual.Jump(targetPos, () =>
        {
            visual.Jump(0.3f, 0.4f);
            this.Delay(0.6f, () =>
            {
                Explosion.Explode(transform.position, 1.5f,info);
                pool.Release(gameObject);
            });
        });
    }
}
