using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class MenuCursor : MonoBehaviour
{
    public Image image;
    private bool isUsingGamepad = false;

    private void Awake()
    {
        Cursor.visible = false;
        InputSystem.onAnyButtonPress.Call(x =>
        {
            if (x.device is Mouse || x.device is Keyboard) isUsingGamepad = false;
            else isUsingGamepad = true;
        });
    }
    private void Update()
    {
        if (isUsingGamepad)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            transform.position = Input.mousePosition;
        }
    }
}
