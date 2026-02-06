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
    [SerializeField] private TMP_Dropdown _resolutionDropdown;

    [SerializeField] private Toggle _fullScreenToggle;
    [SerializeField] private Toggle _windowedToggle;

    [SerializeField] private Button _saveButton;

    public void Init()
    {
        _bgmSlider.value = SoundManager.Instance.BGMVolume;
        _sfxSlider.value = SoundManager.Instance.SFXVolume;

        UpdateBGMText(_bgmSlider.value);
        UpdateSFXText(_sfxSlider.value);

        InitLanguageDropDown();
        InitResolutionDropDown();
        InitScreenModeToggles();

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
            DataManager.SaveScreenData(_resolutionDropdown.value, ScreenManager.Instance.IsFull);

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

    private void InitResolutionDropDown()
    {
        if (_resolutionDropdown == null) return;

        _resolutionDropdown.onValueChanged.RemoveAllListeners();

        _resolutionDropdown.value = ScreenManager.Instance.CurResIndex;

        _resolutionDropdown.onValueChanged.AddListener(index =>
        {
            SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
            ScreenManager.Instance.SetResolution(index);
        });
    }


    private void InitScreenModeToggles()
    {
        bool isFull = ScreenManager.Instance.IsFull;
        _fullScreenToggle.isOn = isFull;
        _windowedToggle.isOn = !isFull;

        _fullScreenToggle.onValueChanged.RemoveAllListeners();
        _fullScreenToggle.onValueChanged.AddListener(isOn =>
        {
            SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
            if (isOn)
            {
                ScreenManager.Instance.SetScreenMode(true);
            }
        });

        _windowedToggle.onValueChanged.RemoveAllListeners();
        _windowedToggle.onValueChanged.AddListener(isOn =>
        {
            SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
            if (isOn)
            {
                ScreenManager.Instance.SetScreenMode(false);
            }
        });
    }

    private void UpdateBGMText(float val) => _bgmVolumeText.text = Mathf.RoundToInt(val * 100).ToString();
    private void UpdateSFXText(float val) => _sfxVolumeText.text = Mathf.RoundToInt(val * 100).ToString();
}
