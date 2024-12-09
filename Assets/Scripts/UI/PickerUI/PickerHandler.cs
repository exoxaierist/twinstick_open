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
    protected int currentIndex = -1;

    protected bool isOpen = false;
    protected bool isTransitioning = false;

    private void Update()
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            PickerElement picker = pickers[i];
            RectTransform pickerT = picker.transform as RectTransform;
            Vector2 targetPos = new((i - currentIndex) * 15 * Mathf.Max(1,20-Mathf.Abs(i-currentIndex)*1.5f),
                -Mathf.Pow(Mathf.Abs(i - currentIndex),2) * 10);
            pickerT.localPosition = (Vector2)pickerT.localPosition + (targetPos - (Vector2)pickerT.localPosition)*Time.unscaledDeltaTime*10;
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
        GameManager.PauseGame();
        UIManager.main.canPause = false;
        gameObject.SetActive(true);
        group.DOFade(1, 0.3f).SetUpdate(true);
        CreateElements();
        this.DelayFrame(()=>
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            Select(0);
        });
    }

    protected virtual void Close()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        GameManager.ResumeGame();
        group.DOFade(0, 0.3f).SetUpdate(true)
            .OnComplete(() =>
            {
                isTransitioning = false;
                isOpen = false;
                UIManager.main.canPause = true;
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

    protected virtual void Select(int index)
    {
        index = Mathf.Clamp(index, 0, pickers.Count - 1);
        if (currentIndex != index) SoundSystem.Play(SoundSystem.UI_HOVER);

        if(currentIndex>=0) pickers[currentIndex].OnDeselect();
        pickers[index].OnSelect();
        currentIndex = index;

        elementName.text = pickers[index].info.name;
        elementDescription.text = pickers[index].info.description;
    }

    protected virtual void Confirm(PickerElement picker) { }
}
