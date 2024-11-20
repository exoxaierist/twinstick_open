using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopHandler : PickerHandler
{
    public Transform elementParent;
    private VendingMachine ownerMachine;

    public void Open(VendingMachine vending)
    {
        ownerMachine = vending;
        UIManager.main.canPause = false;
        Open();
    }

    protected override void Close()
    {
        if (isTransitioning) return;
        UIManager.main.canPause = true;
        isTransitioning = true;
        Player.main.Sleep(false);
        group.DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                isTransitioning = false;
                gameObject.SetActive(false);
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
}
