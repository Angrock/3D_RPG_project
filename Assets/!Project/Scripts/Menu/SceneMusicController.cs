using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SimpleSceneMusic : MonoBehaviour
{
    [Header("Playlist")]
    [SerializeField] private List<AudioClip> tracks = new();
    [SerializeField] private AudioMixerGroup outputGroup;

    [Header("Playback")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private bool shuffle = false;
    [SerializeField] private bool loopPlaylist = true;

    private AudioSource _source;
    private int _currentIndex = 0;
    private Coroutine _playbackCoroutine;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.outputAudioMixerGroup = outputGroup;
        _source.volume = 0f;
    }

    void Start() => PlayPlaylist();

    public void PlayPlaylist()
    {
        if (tracks.Count == 0) return;
        if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
        _currentIndex = shuffle ? Random.Range(0, tracks.Count) : 0;
        _playbackCoroutine = StartCoroutine(PlaybackLoop());
    }

    public void StopPlaylist()
    {
        if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
        StartCoroutine(FadeOutCurrent());
    }

    public void PlayNextTrack()
    {
        if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
        AdvanceIndex();
        _playbackCoroutine = StartCoroutine(PlaybackLoop());
    }

    public void SetVolume(float linearVolume)
    {
        _source.volume = Mathf.Clamp01(linearVolume);
    }

    public bool IsPlaying => _source != null && _source.isPlaying;

    private void AdvanceIndex()
    {
        if (tracks.Count <= 1) return;
        _currentIndex = shuffle ? Random.Range(0, tracks.Count) : (_currentIndex + 1) % tracks.Count;
    }

    private IEnumerator PlaybackLoop()
    {
        int playedCount = 0;
        int limit = loopPlaylist ? int.MaxValue : tracks.Count;

        while (playedCount < limit)
        {
            if (_currentIndex >= tracks.Count)
            {
                _currentIndex = 0;
                if (shuffle) _currentIndex = Random.Range(0, tracks.Count);
            }

            yield return StartCoroutine(FadeAndPlay(tracks[_currentIndex]));
            playedCount++;
            AdvanceIndex();
        }
    }

    private IEnumerator FadeAndPlay(AudioClip clip)
    {
        if (_source.isPlaying)
            yield return StartCoroutine(FadeVolume(_source.volume, 0f, fadeOutDuration));

        _source.clip = clip;
        _source.Play();
        yield return StartCoroutine(FadeVolume(0f, 1f, fadeInDuration));

        while (_source.isPlaying)
            yield return null;
    }

    private IEnumerator FadeOutCurrent()
    {
        yield return StartCoroutine(FadeVolume(_source.volume, 0f, fadeOutDuration));
        _source.Stop();
    }

    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _source.volume = to;
    }
}