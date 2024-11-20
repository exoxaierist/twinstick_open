using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract();
    public void Show();
    public void Hide();
    public void InspectStart();
    public void InspectEnd();
    public void Interact();
}
