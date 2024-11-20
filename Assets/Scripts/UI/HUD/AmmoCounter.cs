using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    public GameObject prefab;
    public List<Image> elements = new();
    public Sprite emptySprite;
    public Sprite fullSprite;

    private int magCount;
    private int ammoCount;

    public void SetMagazineCount(int _magCount)
    {
        if (_magCount == elements.Count) return;
        if (elements.Count > 0 && _magCount < elements.Count)
        {
            for (int i = _magCount; i < elements.Count; i++)
            {
                Destroy(elements[i].gameObject);
            }
        }
        for (int i = elements.Count; i < _magCount; i++)
        {
            Image instance = Instantiate(prefab).GetComponent<Image>();
            instance.transform.SetParent(transform);
            instance.transform.localRotation = Quaternion.Euler(Vector3.zero);
            instance.transform.localScale = Vector3.one;
            elements.Add(instance);
            instance.sprite = emptySprite;
        }
        magCount = _magCount;
    }

    public void SetCount(int newAmmoCount)
    {
        if (ammoCount == newAmmoCount) return;
        if (ammoCount < newAmmoCount)
        {
            for (int i = ammoCount; i < newAmmoCount; i++)
            {
                elements[i].sprite = fullSprite;
            }
        }
        else
        {
            for (int i = ammoCount-1; i > newAmmoCount-1; i--)
            {
                elements[i].sprite = emptySprite;
            }
        }

        if (ammoCount > 0)
        {
            elements[ammoCount-1].DOKill();
            elements[ammoCount-1].rectTransform.DOAnchorPosX(0, 0.2f);
        }
        if(newAmmoCount > 0)
        {
            elements[newAmmoCount - 1].DOKill(); ;
            elements[newAmmoCount - 1].rectTransform.DOAnchorPosX(-15, 0.2f);
        }
        ammoCount = newAmmoCount;
    }
}
