using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class UILocale : MonoBehaviour
{
    public string key = "";
    public UnityEvent<string> onLocaleChange;

    private void Awake() => Locale.onLocaleChange += UpdateLocale;
    private void OnDestroy() => Locale.onLocaleChange -= UpdateLocale;
    private void OnValidate() => UpdateLocale();

    private void Start()
    {
        UpdateLocale();
    }

    [ContextMenu("Update Locale")]
    private void UpdateLocale()
    {
        if (!Locale.ContainsKey(key)) return;
        onLocaleChange?.Invoke(Locale.Get(key));
    }
}
