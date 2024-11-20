using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public static PlayerStart start;
    public bool isStart = false;
    public float spawnDelay = 0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private void Awake()
    {
        if (!isStart) return;
        SetStart();
    }

    public void SetStart()
    {
        if (start != null) return;
        start = this;
    }

    public static void SpawnCamera()
    {
        CamController.main.followTarget = start.transform;
        CamController.main.SetPosition(start.transform.position);
    }

    public static void SpawnPlayerLobby()
    {
        SpawnCamera();

        start.Delay(start.spawnDelay, () =>
        {
            SpawnPlayer();
        });
    }

    public static void SpawnPlayer()
    {
        if (Player.main != null || start == null) return;

        SpriteRenderer spawnEffect = Instantiate(Prefab.Get("PlayerSpawnEffect")).GetComponent<SpriteRenderer>();
        spawnEffect.transform.position = start.transform.position+Vector3.down*0.4f;
        spawnEffect.transform.DOScaleX(0.9f, 0.3f);
        spawnEffect.DOFade(0, 0.3f);

        start.InternalSpawnPlayer();
    }

    public static void DespawnPlayer()
    {
        if (Player.main == null) return;
        Destroy(Player.main.gameObject);
    }

    private void InternalSpawnPlayer()
    {
        GameObject player = Instantiate(Prefab.Get("Player"));
        SpriteRenderer sprite = player.GetComponentInChildren<SpriteRenderer>();

        player.transform.position = start.transform.position;
        CamController.main.followTarget = player.transform;

        sprite.material.SetFloat("_Solidity", 1);
        DOTween.To(() => sprite.material.GetFloat("_Solidity"), x => sprite.material.SetFloat("_Solidity", x), 0, 0.7f);
    }
}
