using System.Collections;
using UnityEngine;

public class KnightEnemy : Enemy
{
    private Collider2D col;

    private Vector2 randomOffset;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        col = GetComponent<Collider2D>();

        nav.optimizePath = false;
        nav.neighborOffsets = new Vector3Int[]
        {
            new(1,2,0),
            new(-1,2,0),
            new(2,1,0),
            new(2,-1,0),
            new(1,-2,0),
            new(-1,-2,0),
            new(-2,1,0),
            new(-2,-1,0)
        };
        randomOffset = Random.insideUnitCircle * 0.4f;
    }

    protected override void OnActivation()
    {
        StartCoroutine(Move());
    }

    protected override void OnDeath(AttackInfo info)
    {
        base.OnDeath(info);
        StopAllCoroutines();
    }

    private IEnumerator Move()
    {
        while (enabled)
        {
            nav.FindPath(Player.main.transform.position);
            pawn.Jump(nav.GetDirection() + randomOffset, 0.4f);

            this.Delay(0.1f, () => { visual.sprite.sortingLayerName = "Overlay"; col.enabled = false; });
            this.Delay(0.3f, () => { visual.sprite.sortingLayerName = "Default"; col.enabled = true; });
            
            SoundSystem.Play(SoundSystem.ACTION_JUMP, transform.position, 0.5f);
            SetSpriteDirection(SpriteDirMode.FacePlayer);
            yield return Wait.Get(1.6f);
        }
    }
}
