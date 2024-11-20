using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentPlayerState : MonoBehaviour
{
    public static PersistentPlayerState main;

    public bool doSpawnPlayerWeapon = true;
    public string playerWeapon;

    private void Awake()
    {
        //if (main != null) { Destroy(gameObject); return; }

        //DontDestroyOnLoad(gameObject);
        main = this;
        Player.onPlayerSpawn += SetPlayerWeapon;
    }

    public void SetPlayerWeapon()
    {
        if (!doSpawnPlayerWeapon || playerWeapon == "" || Player.main == null) return;
        Player.main.SetWeapon(playerWeapon);
    }
}
