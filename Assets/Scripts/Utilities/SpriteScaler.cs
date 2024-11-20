#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpriteScaler : EditorWindow
{
    private float scaleFactor;
    private Texture2D texture;

    [MenuItem("Tools/Scale Sprite Slices")]
    private static void ShowWindow()
    {
        var window = GetWindow<SpriteScaler>();
        window.titleContent = new GUIContent("Scale Sprite Slices");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Double Sprite Slices", EditorStyles.boldLabel);
        texture = (Texture2D)EditorGUILayout.ObjectField("Sprite Atlas", texture, typeof(Texture2D), false);
        scaleFactor = (float)EditorGUILayout.FloatField("Scale Factor", scaleFactor);

        if (texture != null && GUILayout.Button("Scale"))
        {
            DoubleSlices(texture);
        }
    }

    private void DoubleSlices(Texture2D texture)
    {
        if (scaleFactor == 0) return;
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
        {
            SpriteDataProviderFactories factory = new();
            factory.Init();
            ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();

            ScaleSprites(dataProvider);

            dataProvider.Apply();
            var assetImporter = dataProvider.targetObject as AssetImporter;
            assetImporter.SaveAndReimport();
        }
    }

    private void ScaleSprites(ISpriteEditorDataProvider dataProvider)
    {
        SpriteRect[] spriteRects = dataProvider.GetSpriteRects();

        foreach (SpriteRect sprRect in spriteRects)
        {
            Rect rect = sprRect.rect;

            rect.x *= scaleFactor;
            rect.y *= scaleFactor;
            rect.width *= scaleFactor;
            rect.height *= scaleFactor;

            sprRect.rect = rect;
        }

        dataProvider.SetSpriteRects(spriteRects);

        dataProvider.Apply();
    }
}
#endif