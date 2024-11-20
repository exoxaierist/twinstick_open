using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class ShopItem : MonoBehaviour, IInteractable
{
    private static GameObjectPool uiPool = new("ShopItemUI");
    
    [Header("Shop Item")]
    public ItemDisplayInfo info;

    private DescriptionUI ui;
    private bool cancelFlag = false;

    public bool CanInteract() => true;
    public void Hide() { }
    public void Show() { }
    public void InspectStart() => ShowUI();
    public void InspectEnd() => HideUI();
    public void Interact() => OnInteract();

    protected virtual void OnBuy() { }

    private void OnInteract()
    {
        if (Player.coinCount >= info.price) Buy();
        else CannotBuy();
    }

    protected virtual void Buy()
    {
        Player.coinCount -= info.price;
        OnBuy();
        //remove item and add sold or something
        Effect.Play("Pop", EffectInfo.Pos(transform.position+Vector3.up*0.1f));
        HideUI();
        Destroy(gameObject);
    }

    protected void CannotBuy()
    {
        ui.Shake();
    }

    private void ShowUI()
    {
        cancelFlag = true;
        GameObject instance = uiPool.Get();
        ui = instance.GetComponent<DescriptionUI>();
        //ui.SetFloater(transform.position, info);
    }

    private void HideUI()
    {
        ui.Hide();
        cancelFlag = false;
        this.Delay(0.1f,()=> {
            if (cancelFlag) 
            {
                cancelFlag = false;
                return;
            } 
            uiPool.Release(ui.gameObject);
            ui = null;
        });
    }
}
