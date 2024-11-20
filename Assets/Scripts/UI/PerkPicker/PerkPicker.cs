using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkPicker : MonoBehaviour
{
    public static PerkPicker main;
    private static bool canPick = true;

    //entry
    public static void Pick(Action _onPickFinish = null)
    {
        UIManager.main.canPause = false;
        GameManager.PauseGame();
        main.gameObject.SetActive(true);

        main.CreatePickerUI();
        main.ShowTab();
        EventSystem.current.SetSelectedGameObject(main.pickElements[0].gameObject);
        main.onPickFinish = _onPickFinish;
    }

    public TextMeshProUGUI titleText;
    public Transform perkElementParent;
    public RectTransform discardElementParent;
    private GameObject lastSelected;
    private bool tabOpen = false;
    public TextMeshProUGUI perkNameText;
    public TextMeshProUGUI perkDescText;
    public TextMeshProUGUI perkLevelText;
    public Selectable confirmButton;

    public Perk pickedPerk;
    public Perk pickedDiscard;
    public PerkToPick currentPick;
    public PerkToDiscard currentDiscard;
    public int currentDiscardIndex;
    public TextMeshProUGUI chooseDiscardText;
    private CanvasGroup group;

    public Action onDiscardEnabled;
    public Action onDiscardDisabled;
    public Action onPickFinish;

    public bool discardMode = false;

    private List<PerkToPick> pickElements = new();
    private List<PerkToDiscard> discardElements = new();
    private List<GameObject> discardEmptyElements = new();

    private void Awake()
    {
        main = this;
    }

    public void SelectPerk(PerkToPick pick)
    {  
        if(currentPick != null && currentPick != pick) currentPick.Unpick();
        currentPick = pick;
        if (pick == null) return;
        pickedPerk = pick.perk;

        if (Player.CanAddPerk(pickedPerk))
        {
            DisableDiscard();
            confirmButton.interactable = true;
        }
        else
        {
            EnableDiscard();
        }
    }

    public void SelectDiscard(PerkToDiscard pick)
    {
        if(currentDiscard != null && currentDiscard != pick) currentDiscard.Unpick();
        currentDiscard = pick;
        if (pick == null) return;
        pickedDiscard = pick.perk;
        confirmButton.interactable = true;
    }

    private void EnableDiscard()
    {
        if (discardMode) return;
        discardMode = true;
        confirmButton.interactable = false;
        titleText.text = Locale.Get("UI_CHOOSE_DISCARD");
        titleText.color = ColorLib.highlightPink;
        EventSystem.current.SetSelectedGameObject(discardElements[0].gameObject);
        chooseDiscardText.enabled = true;
        onDiscardEnabled?.Invoke();
    }

    private void DisableDiscard()
    {
        if (!discardMode) return;
        titleText.text = Locale.Get("UI_CHOOSE_PERK");
        titleText.color = ColorLib.lightBlueGray;
        discardMode = false;
        currentDiscard = null;
        chooseDiscardText.enabled = false;
        onDiscardDisabled?.Invoke();
    }

    public void ShowTab()
    {
        discardElementParent.DOKill();
        discardElementParent.DOAnchorPosY(380, 0.8f).SetUpdate(true).SetEase(Ease.OutQuint);
    }

    public void Confirm()
    {
        if (discardMode)
        {
            if (currentPick == null || currentDiscard == null) return;
            Player.RemoveAndAddPerk(pickedDiscard, pickedPerk);
        }
        else
        {
            if (currentPick == null) return;
            Player.AddPerk(pickedPerk);
        }

        RemovePickerUI();
        UIManager.main.canPause = true;
        GameManager.ResumeGame();
        this.Delay(0.5f, () => onPickFinish?.Invoke());
        this.Delay(0.5f, () => gameObject.SetActive(false));
    }

    private void CreatePickerUI()
    {
        group = GetComponent<CanvasGroup>();
        group.interactable = true;
        group.blocksRaycasts = true;
        group.DOFade(1, 0.3f).SetUpdate(true);
        chooseDiscardText.enabled = false;

        List<string> perkIDList = Player.GetPerkIDList();
        for (int i = 0; i < 3; i++)
        {
            PerkToPick instance = Instantiate(Prefab.Get("PerkToPick")).GetComponent<PerkToPick>();
            instance.transform.SetParent(perkElementParent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.Set(Perk.GetRandomID(perkIDList),this,i);
            
            pickElements.Add(instance);
            perkIDList.Add(instance.perkID);
        }

        for (int i = 0; i < Player.perks.Count; i++)
        {
            PerkToDiscard instance = Instantiate(Prefab.Get("PerkToDiscard")).GetComponent<PerkToDiscard>();
            instance.transform.SetParent(discardElementParent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.Set(i, Player.perks[i], this);

            discardElements.Add(instance);
        }
        for (int i = Player.perks.Count; i < 10; i++)
        {
            GameObject instance = Instantiate(Prefab.Get("PerkToDiscardEmpty"));
            instance.transform.SetParent(discardElementParent);
            instance.transform.localScale = Vector3.one;
            instance.transform.localPosition = Vector3.zero;
            discardEmptyElements.Add(instance);
        }
        perkNameText.text = "";
        perkDescText.text = "";
        perkLevelText.text = "";
        LayoutRebuilder.ForceRebuildLayoutImmediate(discardElementParent);
        discardElementParent.anchoredPosition = new(0, -20);
    }

    private void RemovePickerUI()
    {
        group.alpha = 1;
        group.interactable = false;
        group.blocksRaycasts = false;
        group.DOFade(0, 0.3f).SetUpdate(true);

        for (int i = 0; i < pickElements.Count; i++)
        {
            pickElements[i].FadeRemove();
        }
        for (int i = 0; i < discardElements.Count; i++)
        {
            discardElements[i].FadeRemove();
        }
        this.DelayRealtime(0.3f, () =>
        {
            for (int i = 0; i < discardEmptyElements.Count; i++)
            {
                Destroy(discardEmptyElements[i]);
            }
            pickElements.Clear();
            discardElements.Clear();
            discardEmptyElements.Clear();
        });
    }
}
