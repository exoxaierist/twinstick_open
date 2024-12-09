using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        main.tabAction.Enable();
        //main.ShowTab();
        EventSystem.current.SetSelectedGameObject(main.pickElements[0].gameObject);
        main.onPickFinish = _onPickFinish;
    }

    public TextMeshProUGUI titleText;
    public Transform perkElementParent;
    public RectTransform discardElementParent;
    public RectTransform tabTransform;
    private GameObject lastSelected;
    private bool tabOpen = false;
    public TextMeshProUGUI perkNameText;
    public TextMeshProUGUI perkDescText;
    public TextMeshProUGUI perkLevelText;
    public Selectable confirmButton;
    public TextMeshProUGUI coinText;
    public Selectable expandButton;
    public TextMeshProUGUI expandBtnText;
    private int expandPrice = 100;

    private bool canReroll = true;
    public Selectable rerollButton;
    public TextMeshProUGUI rerollText;
    private int rerollPrice = 10;

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

    private PlayerInputActionMap inputMap;
    private InputAction tabAction;

    private void Awake()
    {
        main = this;
        inputMap = new PlayerInputActionMap();
        tabAction = inputMap.InGame.Tab;
        tabAction.performed += OnTabAction;
    }

    private void Update()
    {
        if (Player.perks.Count > 15 && Input.mouseScrollDelta.y != 0)
        {
            Vector2 anchorPos = discardElementParent.anchoredPosition;
            anchorPos.y = Mathf.Clamp(anchorPos.y - Input.mouseScrollDelta.y*30, 0, (Player.maxPerkCount / 5) * 170 - 500);
            discardElementParent.anchoredPosition = anchorPos;
        }
    }

    private void OnTabAction(InputAction.CallbackContext ctx)
    {
        if (tabOpen) HideTab(); else ShowTab();
    }

    public void Reroll()
    {
        if (!canReroll) return;
        Player.coinCount -= rerollPrice;

        SelectPerk(null);
        SelectDiscard(null);

        for (int i = 0; i < pickElements.Count; i++)
        {
            pickElements[i].FadeRemove(i*0.05f) ;
        }
        pickElements.Clear();

        this.DelayRealtime(0.45f, () =>
        {
            List<string> perkIDList = Player.GetPerkIDList();
            for (int i = 0; i < 3; i++)
            {
                PerkToPick instance = Instantiate(Prefab.Get("PerkToPick")).GetComponent<PerkToPick>();
                instance.transform.SetParent(perkElementParent);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localScale = Vector3.one;
                instance.Set(Perk.GetRandomID(perkIDList), this, i);

                pickElements.Add(instance);
                perkIDList.Add(instance.perkID);
                canReroll = true;
            }
        });

        rerollPrice += 10;
        rerollText.text = Locale.Get("UI_REROLL") + " <sprite name=\"coin\"> " + rerollPrice;
        coinText.text = "<sprite name=\"coin\"> " + Player.coinCount.ToString();

        if (rerollPrice > Player.coinCount) {
            this.DelayRealtime(0.46f, () => { pickElements[0].Select(); });
            rerollButton.interactable = false; }
        if (expandPrice > Player.coinCount) expandButton.interactable = false;
    }

    public void ExpandSlots()
    {
        Player.coinCount -= expandPrice;
        coinText.text = "<sprite name=\"coin\"> " + Player.coinCount.ToString();
        expandBtnText.text = Locale.Get("UI_EXPAND") + " <sprite name=\"coin\"> " + expandPrice;
        Player.maxPerkCount += 5;
        expandPrice += 50;
        for (int i = 0; i < 5; i++)
        {
            GameObject instance = Instantiate(Prefab.Get("PerkToDiscardEmpty"));
            instance.transform.SetParent(discardElementParent);
            instance.transform.localScale = Vector3.one;
            instance.transform.localPosition = Vector3.zero;
            discardEmptyElements.Add(instance);
        }
        DisableDiscard();
        if (currentPick != null) confirmButton.interactable = true;

        if (rerollPrice > Player.coinCount) rerollButton.interactable = false;
        if (expandPrice > Player.coinCount || Player.maxPerkCount >= Player.maxMaxPerkCount) {
            pickElements[0].Select();
            expandButton.interactable = false; }
    }

    public void SelectPerk(PerkToPick pick)
    {  
        if(currentPick != null && currentPick != pick) currentPick.Unpick();
        currentPick = pick;
        if (pick == null) { confirmButton.interactable = false;  return; }
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
        ShowTab();
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
        if (tabOpen) return;
        tabOpen = true;
        perkElementParent.GetComponent<CanvasGroup>().interactable = false;
        perkElementParent.GetComponent<CanvasGroup>().blocksRaycasts = false;
        tabTransform.GetComponent<CanvasGroup>().interactable = true;
        tabTransform.DOKill();
        tabTransform.DOAnchorPosY(600, 0.4f).SetUpdate(true).SetEase(Ease.InOutExpo);
        if(discardElements.Count>0) EventSystem.current.SetSelectedGameObject(discardElements[0].gameObject);
    }

    public void HideTab()
    {
        if (!tabOpen) return;
        tabOpen = false;
        perkElementParent.GetComponent<CanvasGroup>().blocksRaycasts = true;
        perkElementParent.GetComponent<CanvasGroup>().interactable = true;
        tabTransform.GetComponent<CanvasGroup>().interactable = false;
        tabTransform.DOKill();
        tabTransform.DOAnchorPosY(-10, 0.4f).SetUpdate(true).SetEase(Ease.InOutExpo);
        discardElementParent.DOKill();
        discardElementParent.DOAnchorPosY(0, 0.1f).SetUpdate(true);
        EventSystem.current.SetSelectedGameObject(pickElements[0].gameObject);
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
        main.tabAction.Disable();
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
        titleText.text = Locale.Get("UI_CHOOSE_PERK");
        titleText.color = ColorLib.lightBlueGray; 

        perkElementParent.GetComponent<CanvasGroup>().interactable = true;
        perkElementParent.GetComponent<CanvasGroup>().blocksRaycasts = true;
        tabTransform.GetComponent<CanvasGroup>().interactable = false;

        rerollPrice = 10;
        coinText.text = "<sprite name=\"coin\"> " + Player.coinCount.ToString();
        if (rerollPrice > Player.coinCount) rerollButton.interactable = false; else rerollButton.interactable = true;
        if (expandPrice > Player.coinCount || Player.maxPerkCount >= Player.maxMaxPerkCount) expandButton.interactable = false; else expandButton.interactable = true;
        rerollText.text = Locale.Get("UI_REROLL") + " <sprite name=\"coin\"> " + rerollPrice;
        expandBtnText.text = Locale.Get("UI_EXPAND") + " <sprite name=\"coin\"> " + expandPrice;

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
        for (int i = Player.perks.Count; i < Player.maxPerkCount; i++)
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabTransform);
        tabTransform.anchoredPosition = new(0, -20);
    }

    private void RemovePickerUI()
    {
        group.alpha = 1;
        group.interactable = false;
        group.blocksRaycasts = false;
        group.DOFade(0, 0.3f).SetUpdate(true);

        discardMode = false;
        pickedPerk = null;
        pickedDiscard = null;
        currentDiscard = null;
        currentPick = null;

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
