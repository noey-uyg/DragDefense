using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : PopupBase
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private TextMeshProUGUI _bgmVolumeText;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _sfxVolumeText;

    [SerializeField] private TMP_Dropdown _languageDropdown;

    [SerializeField] private Button _saveButton;

    public void Init()
    {
        _bgmSlider.value = SoundManager.Instance.BGMVolume;
        _sfxSlider.value = SoundManager.Instance.SFXVolume;

        UpdateBGMText(_bgmSlider.value);
        UpdateSFXText(_sfxSlider.value);

        InitLanguageDropDown();

        _bgmSlider.onValueChanged.RemoveAllListeners();
        _bgmSlider.onValueChanged.AddListener(val =>
        {
            SoundManager.Instance.SetBGMVolume(val);
            UpdateBGMText(val);
        });

        _sfxSlider.onValueChanged.RemoveAllListeners();
        _sfxSlider.onValueChanged.AddListener(val =>
        {
            SoundManager.Instance.SetSFXVolume(val);
            UpdateSFXText(val);
        });

        _saveButton.onClick.RemoveAllListeners();
        _saveButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
            DataManager.SaveSoundData(_bgmSlider.value, _sfxSlider.value);
            DataManager.SaveLanguageData((Language)_languageDropdown.value);
            
            PopupManager.Instance.HideTopPopup();
            GameManager.Instance.SetGameState(GameState.Lobby);
        });
    }

    private void InitLanguageDropDown()
    {
        if (_languageDropdown == null) return;

        _languageDropdown.onValueChanged.RemoveAllListeners();

        _languageDropdown.value = (int)LocalizationManager.Instance.CurLanguage;

        _languageDropdown.onValueChanged.AddListener(index =>
        {
            SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
            LocalizationManager.Instance.ChangeLanguage((Language)index);
        });
    }

    private void UpdateBGMText(float val) => _bgmVolumeText.text = Mathf.RoundToInt(val * 100).ToString();
    private void UpdateSFXText(float val) => _sfxVolumeText.text = Mathf.RoundToInt(val * 100).ToString();
}
