using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickerHandler : Selectable, IMoveHandler, ISubmitHandler, ICancelHandler
{
    public CanvasGroup group;

    public TextMeshProUGUI elementName;
    public TextMeshProUGUI elementDescription;
    public TextMeshProUGUI elementPrice;
    
    public List<PickerElement> pickers = new();
    int currentIndex = -1;

    protected bool isOpen = false;
    protected bool isTransitioning = false;

    private void Update()
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            PickerElement picker = pickers[i];
            RectTransform pickerT = picker.transform as RectTransform;
            Vector2 targetPos = new((i - currentIndex) * 15 * Mathf.Max(1,20-Mathf.Abs(i-currentIndex)*2f),
                -Mathf.Pow(Mathf.Abs(i - currentIndex),2) * 10);
            pickerT.localPosition = (Vector2)pickerT.localPosition + (targetPos - (Vector2)pickerT.localPosition)*Time.deltaTime*10;
        }
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Right) Select(currentIndex + 1);
        else if (eventData.moveDir == MoveDirection.Left) Select(currentIndex - 1);
    }

    void ISubmitHandler.OnSubmit(BaseEventData eventData)
    {
        Confirm(pickers[currentIndex]);
    }

    public void OnCancel(BaseEventData eventData)
    {
        Close();
    }

    protected void Open()
    {
        if (isTransitioning) return;
        isOpen = true;
        Player.main.Sleep(true);
        gameObject.SetActive(true);
        group.DOFade(1, 0.3f);
        CreateElements();
        this.DelayFrame(()=>EventSystem.current.SetSelectedGameObject(gameObject));
        Select(0);
    }

    protected virtual void Close()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        Player.main.Sleep(false);
        group.DOFade(0, 0.3f)
            .OnComplete(() =>
            {
                isTransitioning = false;
                isOpen = false;
                gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                for (int i = 0; i < pickers.Count; i++)
                {
                    Destroy(pickers[i].gameObject);
                }
                pickers.Clear();
            });
    }

    protected virtual void CreateElements() { }

    private void Select(int index)
    {
        index = Mathf.Clamp(index, 0, pickers.Count - 1);
        if (index == currentIndex) return;

        elementName.text = pickers[index].info.name;
        elementDescription.text = pickers[index].info.description;
        pickers[index].OnSelect();
        if(currentIndex>=0) pickers[currentIndex].OnDeselect();
        currentIndex = index;
    }

    protected virtual void Confirm(PickerElement picker)
    {
        
    }
}
