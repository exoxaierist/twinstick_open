using TMPro;
using UnityEngine;

public class InteractPopup : MonoBehaviour
{
    public static GameObjectPool popupPool = new("InteractPopup");

    public static InteractPopup Get(string text, Vector3 position, Vector3 offset)
    {
        InteractPopup instance = popupPool.Get().GetComponent<InteractPopup>();
        instance.transform.SetParent(UIManager.main.shopUIParent);
        instance.transform.localScale = Vector3.one;
        instance.text.text = text;
        instance.position = position;
        instance.offset = offset;
        return instance;
    }
    public static void Release(InteractPopup instance)
    {
        popupPool.Release(instance.gameObject);
    }

    public TextMeshProUGUI text;
    public Vector3 position;
    public Vector3 offset;

    private void Update()
    {
        Vector3 halfScreen = new(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        transform.position = (Camera.main.WorldToScreenPoint(position) - halfScreen) * 1.05f + halfScreen + offset;
    }
}
