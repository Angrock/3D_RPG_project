using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

[System.Serializable]
public class CharacterSoundDef
{
    public string id;
    public AudioClip clip;
    public AudioMixerGroup group;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
    public bool is3D = true;
    public float cooldownSec = 0.15f;
    public int priority = 0;
    public bool isLoop = false; // ← Новый флаг: помечает звуки, которые должны лупиться по умолчанию
}

public class CharacterAudioController : MonoBehaviour
{
    [SerializeField] private List<CharacterSoundDef> sounds;
    [SerializeField] private int poolSize = 6;

    private List<AudioSource> _pool = new();
    private Dictionary<string, float> _lastPlay = new();
    private Dictionary<string, AudioSource> _activeLoops = new(); // Трекинг лупов по id

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.bypassEffects = false;
            src.spatialBlend = 1f;
            _pool.Add(src);
        }
    }

    // === РАЗОВЫЕ ЗВУКИ ===
    public void Play(string id, float overrideVolume = 1f, float overridePitch = 1f)
    {
        var def = sounds.Find(s => s.id == id);
        if (def == null) return;

        if (_lastPlay.TryGetValue(id, out float last) && Time.unscaledTime - last < def.cooldownSec)
            return;
        _lastPlay[id] = Time.unscaledTime;

        var src = _pool.Find(s => !s.isPlaying) ?? _pool[Random.Range(0, _pool.Count)];
        
        src.clip = def.clip;
        src.outputAudioMixerGroup = def.group;
        src.volume = def.volume * Mathf.Clamp01(overrideVolume);
        src.pitch = Mathf.Clamp(def.pitch * overridePitch, 0.5f, 2f); // ← безопасный клэмп итогового значения
        src.spatialBlend = def.is3D ? 1f : 0f;
        src.loop = false; // Явно отключаем луп для разовых звуков
        src.Play();
    }

    // === ЗАЦИКЛЕННЫЕ ЗВУКИ ===
    public void PlayLoop(string id, float overrideVolume = 1f, float overridePitch = 1f)
    {
        var def = sounds.Find(s => s.id == id);
        if (def == null) return;

        if (_activeLoops.ContainsKey(id))
            return;

        var src = _pool.Find(s => !s.isPlaying);
        if (src == null)
        {
            Debug.LogWarning($"[Audio] Pool exhausted for loop '{id}'. Consider increasing poolSize or separating loop pool.");
            return;
        }

        src.clip = def.clip;
        src.outputAudioMixerGroup = def.group;
        src.volume = def.volume * Mathf.Clamp01(overrideVolume);
        src.pitch = Mathf.Clamp(def.pitch * overridePitch, 0.5f, 2f);
        src.spatialBlend = def.is3D ? 1f : 0f;
        src.loop = true; // ← Ключевое: включаем зацикливание
        src.Play();

        _activeLoops[id] = src;
    }

    public void Stop(string id)
    {
        // 1. Пробуем остановить как луп
        if (_activeLoops.TryGetValue(id, out var loopSrc))
        {
            loopSrc.Stop();
            _activeLoops.Remove(id);
            return;
        }

        var def = sounds.Find(s => s.id == id);
        if (def?.clip != null)
        {
            foreach (var src in _pool)
            {
                if (src.clip == def.clip && src.isPlaying)
                {
                    src.Stop();
                    break;
                }
            }
        }
    }

    public bool IsLoopPlaying(string id) => _activeLoops.ContainsKey(id);

    public void StopAllLoops()
    {
        foreach (var kvp in _activeLoops)
            kvp.Value?.Stop();
        _activeLoops.Clear();
    }

    public void StopLoopWithFade(string id, float fadeDuration = 0.2f)
    {
        if (!_activeLoops.TryGetValue(id, out var src)) return;
        StartCoroutine(FadeAndStop(src, id, fadeDuration));
    }

    private System.Collections.IEnumerator FadeAndStop(AudioSource src, string id, float duration)
    {
        float startVol = src.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        src.Stop();
        src.volume = startVol; // Возвращаем громкость для следующего использования
        _activeLoops.Remove(id);
    }
}