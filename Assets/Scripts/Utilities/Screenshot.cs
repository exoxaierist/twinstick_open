using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public bool capture = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) ScreenCapture.CaptureScreenshot("screenshots/screenshot.png");
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        if (capture)
        {
            print("captured");
            ScreenCapture.CaptureScreenshot("screenshots/screenshot.png");
            capture = false;
        }
    }
}
