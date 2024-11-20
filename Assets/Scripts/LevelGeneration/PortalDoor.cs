using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D col;
    public bool isVertical = true;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    public void Open()
    {
        if(isVertical) animator.Play("PortalDoor_Open_Vertical");
        else animator.Play("PortalDoor_Open_Horizontal");
        this.Delay(0.333f,()=>GetComponent<SpriteRenderer>().sortingLayerName = "FloorOverlay0");
        col.enabled = false;
    }

    public void Lock()
    {
        if (isVertical) animator.Play("PortalDoor_Close_Vertical");
        else animator.Play("PortalDoor_Close_Horizontal");
        GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        col.enabled = true;
    }
}
 