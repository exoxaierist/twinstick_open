using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickerElement :MonoBehaviour
{
    public ItemDisplayInfo info;

    public virtual void OnSpawn()
    {

    }

    public virtual bool CanConfirm()
    {
        return true;
    }

    public virtual void OnConfirm()
    {

    }

    public void OnSelect()
    {
        transform.DOKill();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.DOScale(1.2f, 0.3f);
    }

    public void OnDeselect()
    {
        transform.DOKill();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.DOScale(1, 0.3f);
    }

    public void Shake()
    {
        transform.DOComplete();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.DOPunchRotation(new(0, 0, 10), 0.2f);
    }
}
