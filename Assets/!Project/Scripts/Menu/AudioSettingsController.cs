using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Exposed parameter names (must match AudioMixer)")]
    [SerializeField] private string masterParam = "MasterVol";
    [SerializeField] private string sfxParam = "SFXVol";
    [SerializeField] private string musicParam = "MusicVol";

    [Header("Persistence")]
    [SerializeField] private string prefsKeyPrefix = "AudioVol_";

    void Awake()
    {
        // Инициализация слайдеров: загружаем сохранённые значения
        SetupSlider(masterSlider, masterParam);
        SetupSlider(sfxSlider, sfxParam);
        SetupSlider(musicSlider, musicParam);
    }

    private void SetupSlider(Slider slider, string parameterName)
    {
        if (slider == null) return;
        
        // Загружаем из PlayerPrefs (0..1), по умолчанию 1.0
        float saved = PlayerPrefs.GetFloat(prefsKeyPrefix + parameterName, 1f);
        slider.value = saved;
        
        // Применяем сразу при старте
        ApplyVolume(parameterName, saved);
        
        // Подписываем на изменения
        slider.onValueChanged.AddListener(val => OnSliderChanged(parameterName, val));
    }

    private void OnSliderChanged(string parameterName, float linearValue)
    {
        ApplyVolume(parameterName, linearValue);
        PlayerPrefs.SetFloat(prefsKeyPrefix + parameterName, linearValue);
        PlayerPrefs.Save(); // Можно вынести в отдельный менеджер сохранений
    }

    // Конвертация линейного 0..1 → децибелы -80..0
    public static float LinearToDecibels(float linear)
    {
        if (linear <= 0.0001f) return -80f; // Практическая тишина
        return 20f * Mathf.Log10(linear);
    }

    private void ApplyVolume(string parameterName, float linearValue)
    {
        float db = LinearToDecibels(linearValue);
        mixer.SetFloat(parameterName, db);
    }

    // Опционально: утилита для чтения текущего значения (например, для синхронизации UI)
    public static float DecibelsToLinear(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }
}