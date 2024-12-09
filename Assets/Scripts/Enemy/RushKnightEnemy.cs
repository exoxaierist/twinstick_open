using System.Collections;
using UnityEngine;

public class RushKnightEnemy : Enemy
{
    private Collider2D col;
    private Vector2 randomOffset;
    private Coroutine moveRoutine;

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

    protected override void OnIntervalUpdate()
    {
        if (!isActive) return;
        if(Vector2.Distance(Player.main.transform.position,transform.position) < 8)
        {
            moveRoutine ??= StartCoroutine(Move());
        }
        else
        {
            if(moveRoutine!=null)StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
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
            for (int c = 0; c < 3; c++)
            {
                nav.FindPath(Player.main.transform.position);
                pawn.Jump(nav.GetDirection() + randomOffset, 0.4f);
                SoundSystem.Play(SoundSystem.ACTION_JUMP, transform.position, 0.5f);

                this.Delay(0.1f, () => { visual.sprite.sortingLayerName = "Overlay"; col.enabled = false; });
                this.Delay(0.3f, () => { visual.sprite.sortingLayerName = "Default"; col.enabled = true; });

                //set sprite dir
                SetSpriteDirection(SpriteDirMode.FacePlayer);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(3);
        }
    }
}
