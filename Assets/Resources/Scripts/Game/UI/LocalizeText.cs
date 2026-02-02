using TMPro;
using UnityEngine;

public class LocalizeText : MonoBehaviour
{
    [SerializeField] private string _localizeKey;
    [SerializeField] private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        if (_textMesh == null) _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        LocalizationManager.Instance.RegisterLanguageEvent(UpdateText);
        UpdateText();
    }

    private void OnDisable()
    {
        LocalizationManager.Instance.UnregisterLanguageEvent(UpdateText);
    }

    public void UpdateText()
    {
        if(string.IsNullOrEmpty(_localizeKey)) return;

        _textMesh.text = LocalizationManager.Instance.GetText(_localizeKey);
    }

    public void SetKey(string key)
    {
        _localizeKey = key;
        UpdateText();
    }
}
