#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

[Overlay(typeof(EditorWindow), "Locale Setting")]
public class EditorLocaleSetting : Overlay
{
    public override VisualElement CreatePanelContent()
    {
        VisualElement root = new();

        string[] options = Enum.GetNames(typeof(Language));
        PopupField<string> dropdown = new(options.ToList(), Locale.lang.ToString());

        dropdown.RegisterValueChangedCallback(x=>OnValueChange(x.newValue));
        dropdown.style.width = 100;

        root.Add(dropdown);

        return root;
    }

    private void OnValueChange(string newValue)
    {
        switch (newValue)
        {
            case "English":
                Locale.SetLanguage(Language.English);
                break;
            case "Korean":
                Locale.SetLanguage(Language.Korean);
                break;
        }
    }
}
#endif
