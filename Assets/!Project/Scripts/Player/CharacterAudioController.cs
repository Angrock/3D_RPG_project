using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
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
}

public class CharacterAudioController : MonoBehaviour
{
    [SerializeField] private List<CharacterSoundDef> sounds;
    [SerializeField] private int poolSize = 6;

    private Dictionary<string, CharacterSoundDef> _soundDefs;
    private Dictionary<string, float> _lastPlay;
    private Dictionary<string, AudioSource> _activeLoops;
    private Dictionary<string, AudioSource> _activeOneShots;
    private AudioSource[] _pool;
    private int _poolIndex;

    private void Awake()
    {
        _soundDefs = new Dictionary<string, CharacterSoundDef>(sounds.Count);
        _lastPlay = new Dictionary<string, float>(sounds.Count);
        _activeLoops = new Dictionary<string, AudioSource>();
        _activeOneShots = new Dictionary<string, AudioSource>();

        foreach (var def in sounds)
        {
            if (!string.IsNullOrEmpty(def.id) && def.clip != null)
                _soundDefs[def.id] = def;
        }

        _pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            _pool[i] = gameObject.AddComponent<AudioSource>();
            _pool[i].playOnAwake = false;
            _pool[i].spatialBlend = 1f;
        }
        _poolIndex = 0;
    }

    public void Play(string id, float overrideVolume = 1f, float overridePitch = 1f)
    {
        if (!_soundDefs.TryGetValue(id, out var def)) return;

        if (_lastPlay.TryGetValue(id, out float last) && Time.unscaledTime - last < def.cooldownSec)
            return;

        _lastPlay[id] = Time.unscaledTime;

        if (_activeOneShots.TryGetValue(id, out var prevSrc) && prevSrc.isPlaying)
            prevSrc.Stop();

        var src = GetAvailableSource();
        ApplySourceSettings(src, def, overrideVolume, overridePitch);
        src.loop = false;
        src.Play();

        _activeOneShots[id] = src;
        StartCoroutine(CleanupAfterPlayback(src, id));
    }

    public void PlayLoop(string id, float overrideVolume = 1f, float overridePitch = 1f)
    {
        if (!_soundDefs.TryGetValue(id, out var def)) return;
        if (_activeLoops.ContainsKey(id)) return;

        
        AudioSource src = null;
        for (int i = 0; i < _pool.Length; i++)
        {
            if (!_pool[i].isPlaying)
            {
                src = _pool[i];
                break;
            }
        }

        if (src == null)
        {
            Debug.LogWarning($"[Audio] Pool exhausted for loop '{id}'. Увеличьте poolSize или выделите отдельный пул для луков.");
            return;
        }

        ApplySourceSettings(src, def, overrideVolume, overridePitch);
        src.loop = true;
        src.Play();

        _activeLoops[id] = src;
    }

    public void Stop(string id)
    {
        if (_activeLoops.TryGetValue(id, out var loopSrc))
        {
            loopSrc.Stop();
            _activeLoops.Remove(id);
            return;
        }

        if (_activeOneShots.TryGetValue(id, out var oneShotSrc) && oneShotSrc.isPlaying)
        {
            oneShotSrc.Stop();
            _activeOneShots.Remove(id);
        }
    }

    public bool IsLoopPlaying(string id) => _activeLoops.ContainsKey(id);

    public void StopAllLoops()
    {
        foreach (var src in _activeLoops.Values) src?.Stop();
        _activeLoops.Clear();
    }

    public void StopLoopWithFade(string id, float fadeDuration = 0.2f)
    {
        if (!_activeLoops.TryGetValue(id, out var src)) return;
        StartCoroutine(FadeAndStop(src, id, fadeDuration));
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var src in _pool) if (!src.isPlaying) return src;

        var srcToReuse = _pool[_poolIndex];
        srcToReuse.Stop();
        _poolIndex = (_poolIndex + 1) % _pool.Length;
        return srcToReuse;
    }

    private void ApplySourceSettings(AudioSource src, CharacterSoundDef def, float overrideVolume, float overridePitch)
    {
        src.clip = def.clip;
        src.outputAudioMixerGroup = def.group;
        src.volume = def.volume * Mathf.Clamp01(overrideVolume);
        src.pitch = Mathf.Clamp(def.pitch * overridePitch, 0.5f, 2f);
        src.spatialBlend = def.is3D ? 1f : 0f;
    }

    private IEnumerator FadeAndStop(AudioSource src, string id, float duration)
    {
        float startVol = src.volume;
        float elapsed = 0f;
        
        while (elapsed < duration && src.isPlaying)
        {
            elapsed += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        src.Stop();
        src.volume = startVol;
        _activeLoops.Remove(id);
    }

    private IEnumerator CleanupAfterPlayback(AudioSource src, string id)
    {
        while (src.isPlaying) yield return null;
        _activeOneShots.Remove(id);
    }
}