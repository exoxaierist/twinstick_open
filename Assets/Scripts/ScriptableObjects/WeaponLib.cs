using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponClassLibrary",menuName ="WeaponClassLibrary")]
public class WeaponLib : ScriptableObject
{
    public static WeaponLib _main;
    public static WeaponLib Main
    {
        get { 
            if (_main == null) 
                _main = Resources.Load<WeaponLib>("WeaponLib");
            return _main;
        }
        set { _main = value; }
    }

    public List<WeaponInfo> weaponList;

    public static List<WeaponInfo> GetList() => Main.weaponList;

    public static WeaponInfo Get(string ID)
    {
        //Main.UpdateUnlockState();
        foreach (WeaponInfo info in Main.weaponList)
        {
            if (info.ID == ID) 
                return info;
        }
        return new();
    }

    public static void SetUnlock(string ID, bool unlocked) 
    {
        Main.UpdateUnlockState();
        for (int i = 0; i < Main.weaponList.Count; i++)
        {
            if (Main.weaponList[i].ID == ID)
            {
                WeaponInfo info = Main.weaponList[i];
                info.unlocked = unlocked;
                Main.weaponList[i] = info;
            }
        }
        Main.SaveUnlockState();
    }

    private void UpdateUnlockState()
    {
        string state = Save.GetString("WPUnlock");
        if (state.Length != weaponList.Count)
        {
            SaveUnlockState();
            return;
        }

        for (int i = 0; i < weaponList.Count; i++)
        {
            WeaponInfo info = weaponList[i];
            info.unlocked = state[i] == '1';
            weaponList[i] = info;
        }
    }

    [ContextMenu("Save Unlock State")]
    private void SaveUnlockState()
    {
        StringBuilder newState = new();
        for (int i = 0; i < weaponList.Count; i++) 
            newState.Append(weaponList[i].unlocked ? '1' : '0');
        Save.SaveString("WPUnlock", newState.ToString());
    }
}
