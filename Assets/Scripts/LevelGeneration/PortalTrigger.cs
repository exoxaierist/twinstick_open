using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        UIManager.main.FadeIn(0.4f);
        this.Delay(0.4f, () =>
        {
            LevelManager.main.ChangeRoom(
                GetComponentInParent<MetaRoom>(),
                GetComponentInParent<RoomPortal>().direction);
        });
    }
}
