using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stepper : Selectable
{
    public TextMeshProUGUI text;
    public Graphic arrowLeft;
    public Graphic arrowRight;

    public List<string> options = new();
    private int _value = 0;
    public int value
    {
        get { return _value; }
        set { SetIndex(value); }
    }

    public UnityEvent<int> onValueChange;

    protected override void Start()
    {
        //UpdateVisual();
    }

    public void MoveRight()=> SetIndex(_value + 1);
    public void MoveLeft()=> SetIndex(_value - 1);

    public override void OnMove(AxisEventData eventData)
    {
        base.OnMove(eventData);
        if (eventData.moveDir == MoveDirection.Right) MoveRight();
        else if (eventData.moveDir == MoveDirection.Left) MoveLeft();
    }

    private void SetIndex(int newValue)
    {
        newValue = Mathf.Clamp(newValue, 0, options.Count - 1);
        _value = newValue;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        text.text = options[_value];
        arrowLeft.color = Color.white.WithAlpha(1);
        arrowRight.color = Color.white.WithAlpha(1);
        if (_value == 0) arrowLeft.color = Color.white.WithAlpha(0.2f);
        if (_value == options.Count - 1) arrowRight.color = Color.white.WithAlpha(0.2f);
        onValueChange?.Invoke(_value);
    }
}
