using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
        this.Delay(Random.Range(0, 1f), () => StartCoroutine(Move()));
    }

    private IEnumerator Move()
    {
        while (enabled)
        {
            nav.FindPath(Player.main.transform.position);
            pawn.Jump(nav.GetDirection() + randomOffset, 0.4f);
            //doContactDamage = false;
            //this.Delay(0.4f, () => doContactDamage = true);

            this.Delay(0.1f, () => { visual.sprite.sortingLayerName = "Overlay"; col.enabled = false; });
            this.Delay(0.3f, () => { visual.sprite.sortingLayerName = "Default"; col.enabled = true; });

            //set sprite dir
            SetSpriteDirection(SpriteDirMode.FacePlayer);
            
            yield return new WaitForSeconds(1.6f);
        }
    }
}
