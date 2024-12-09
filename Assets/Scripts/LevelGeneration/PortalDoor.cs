using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D col;
    public bool isVertical = true;
    public bool playin = false;
    public bool playout = false;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    public void Open()
    {
        if(playout)SoundSystem.Play(SoundSystem.MISC_DOOR_CLOSE, transform.position);
        if(isVertical) animator.Play("PortalDoor_Open_Vertical");
        else animator.Play("PortalDoor_Open_Horizontal");
        this.Delay(0.333f,()=>GetComponent<SpriteRenderer>().sortingLayerName = "FloorOverlay0");
        col.enabled = false;
    }

    public void Close()
    {
        if(playin) SoundSystem.Play(SoundSystem.MISC_DOOR_CLOSE, transform.position);
        if (isVertical) animator.Play("PortalDoor_Close_Vertical");
        else animator.Play("PortalDoor_Close_Horizontal");
        GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        col.enabled = true;
    }
}
 