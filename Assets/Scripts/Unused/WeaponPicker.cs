using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class WeaponPicker : MonoBehaviour
{
    private static WeaponPicker main;

    public RectTransform layout;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDescription;
    public TextMeshProUGUI confirmText;
    private Image weaponNameBox;
    public Color unlockedColor;
    public Color lockedColor;

    private List<WeaponInfo> weaponList = new();
    private List<WeaponToPick> picks = new();
    public string weaponSelected = "";

    public int currentIndex = 0;
    private Action<int> selectByIndex;
    private Action<int> onSelectionChange;
    private Action onFinish;

    private Tweener tweener;

    private void Awake()
    {
        main = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (tweener != null) return;
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectByIndex?.Invoke(Mathf.Min(picks.Count-1,++currentIndex));
        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectByIndex?.Invoke(Mathf.Max(0,--currentIndex));
        if (Input.GetKeyDown(KeyCode.Return)) Confirm();
    }

    //entry
    public static void Pick(Action onFinish)
    {
        main.gameObject.SetActive(true);
        main.CreatePickCards();
        main.onFinish = onFinish;
    }

    public void Confirm()
    {
        PersistentPlayerState.main.playerWeapon = weaponSelected;
        gameObject.SetActive(false);
        onFinish?.Invoke();
    }

    public void Select(WeaponInfo weaponInfo, float pos, int index)
    {
        if (tweener != null) return;
        if (weaponSelected == weaponInfo.ID) return;
        weaponSelected = weaponInfo.ID;
        weaponName.text = weaponInfo.name;
        weaponDescription.text = weaponInfo.description;
        weaponNameBox.color = weaponInfo.unlocked ? unlockedColor : lockedColor;
        confirmText.color = weaponInfo.unlocked ? unlockedColor : lockedColor;
        confirmText.text = weaponInfo.unlocked ? "PRESS ENTER TO CONFIRM" : "LOCKED";
        
        tweener = layout.DOAnchorPosY(-pos, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(()=>tweener = null);

        currentIndex = index;
        onSelectionChange?.Invoke(index);
        LayoutRebuilder.ForceRebuildLayoutImmediate(weaponName.transform.parent.transform as RectTransform);
    }

    private void CreatePickCards()
    {
        weaponList = WeaponLib.GetList();
        int unlockedCount = 0;
        for (int i = 0; i < weaponList.Count; i++)
        {
            WeaponInfo info = weaponList[i];
            WeaponToPick instance = Instantiate(Prefab.Get("WeaponToPick")).GetComponent<WeaponToPick>();
            instance.transform.SetParent(layout);
            if (info.unlocked) instance.transform.SetSiblingIndex(unlockedCount);
            else instance.transform.SetAsLastSibling();
            picks.Add(instance);
            if (info.unlocked) unlockedCount++;

            selectByIndex += instance.SelectByIndex;
            onSelectionChange += instance.OnSelectChange;
            instance.Set(this, weaponList[i]);
        }

        weaponNameBox = weaponName.transform.parent.GetComponent<Image>();
    }
}
