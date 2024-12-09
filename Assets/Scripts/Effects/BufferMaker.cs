using UnityEngine;

[ExecuteInEditMode]
public class BufferMaker : MonoBehaviour
{
    public RenderTexture floorMaskBuffer;

    void OnEnable()
    {
        //floorMaskBuffer = new(1920, 1080, 0, RenderTextureFormat.ARGB32);
        floorMaskBuffer.enableRandomWrite = true;
        //floorMaskBuffer.Create();

        //Shader.SetGlobalTexture("_FloorMaskTex", floorMaskBuffer);
    }
}
