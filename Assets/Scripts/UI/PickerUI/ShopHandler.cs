using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopHandler : PickerHandler
{
    public TextMeshProUGUI priceText;
    public Transform elementParent;
    private VendingMachine ownerMachine;

    public void Open(VendingMachine vending)
    {
        ownerMachine = vending;
        Open();
    }

    protected override void Close()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        GameManager.ResumeGame();
        group.DOFade(0, 0.3f).SetUpdate(true)
            .OnComplete(() =>
            {
                isTransitioning = false;
                gameObject.SetActive(false);
                UIManager.main.canPause = true;
                EventSystem.current.SetSelectedGameObject(null);
                ownerMachine.OnInteractEnd();
                ownerMachine = null;
                pickers.Clear();
            });
    }

    protected override void Confirm(PickerElement picker)
    {
        if (picker.CanConfirm())
        {
            picker.OnConfirm();
            Close();
        }
        else
        {
            SoundSystem.Play(SoundSystem.MISC_ERROR);
            picker.Shake();
        }
    }

    protected override void CreateElements()
    {
        foreach (ShopItemElement item in ownerMachine.items)
        {
            item.transform.SetParent(elementParent);
            item.gameObject.SetActive(true);
            item.spawnPos = ownerMachine.transform.position;
            item.OnSpawn();
            pickers.Add(item);
        }
    }

    protected override void Select(int index)
    {
        index = Mathf.Clamp(index, 0, pickers.Count - 1);
        base.Select(index);
        priceText.text = "<sprite name=\"coin\"> " + pickers[index].info.price;
    }
}
