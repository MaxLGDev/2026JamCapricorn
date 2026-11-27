using System;
using System.Collections.Generic;
using UnityEngine;
 
// BGM・効果音を管理するクラス（音量は固定）
public class SoundManager : MonoBehaviour
{
    // シングルトン参照
    public static SoundManager Instance;
 
    // 音データ
    public Sound[] musicSounds, effectSounds;
 
    // 再生用 AudioSource
    public AudioSource musicSource;
    public AudioSource sfxSource;
 
    private readonly Dictionary<string, AudioSource> loopSources = new();
    private bool isMuted;
 
    // シングルトン初期化
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複防止
            return;
        }
 
        Instance = this;
        DontDestroyOnLoad(gameObject); // シーン跨ぎで保持
    }
 
    // BGM再生（同じ曲なら何もしない）
    public void PlayMusic(string name)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is missing", this);
            return;
        }
 
        Sound sound = Find(musicSounds, name);
        if (sound == null)
            return;
 
        if (musicSource.isPlaying && musicSource.clip == sound.audioClip)
            return;
 
        musicSource.Stop();
        musicSource.clip = sound.audioClip;
        musicSource.loop = true;
        musicSource.Play();
    }
 
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
 
    // 効果音再生（ピッチ変更可）
    public void PlaySFX(string name, float pitch = 1f)
    {
        if (sfxSource == null)
        {
            Debug.LogError("SFX Source is missing", this);
            return;
        }
 
        Sound sound = Find(effectSounds, name);
        if (sound == null)
            return;
 
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(sound.audioClip);
    }
 
    // ボタン用ラッパー（InspectorのOnClickから呼べる）
    public void PlaySFX(string name)
    {
        PlaySFX(name, 1f);
    }
 
    // ループ効果音の開始（owner ごとに1つ）
    public void StartLoop(string soundName, string owner)
    {
        Sound sound = Find(effectSounds, soundName);
        if (sound == null)
            return;
 
        if (!loopSources.TryGetValue(owner, out AudioSource source))
        {
            source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxSource != null ? sfxSource.outputAudioMixerGroup : null;
            source.playOnAwake = false;
            source.loop = true;
            loopSources[owner] = source;
        }
 
        if (source.isPlaying && source.clip == sound.audioClip)
            return;
 
        source.clip = sound.audioClip;
        source.Play();
    }
 
    public void StopLoop(string owner)
    {
        if (loopSources.TryGetValue(owner, out AudioSource source))
            source.Stop();
    }
 
    // 全体一時停止・再開
    public void PauseAll() => AudioListener.pause = true;
    public void ResumeAll() => AudioListener.pause = false;
 
    // ミュート切替
    public void ToggleMute() => SetMuted(!isMuted);
 
    public void SetMuted(bool mute)
    {
        isMuted = mute;
        AudioListener.volume = mute ? 0f : 1f;
    }
 
    public bool IsMuted() => isMuted;
 
    // 名前で検索
    private Sound Find(Sound[] sounds, string name)
    {
        Sound sound = sounds == null ? null : Array.Find(sounds, s => s.soundName == name);
 
        if (sound == null)
            Debug.LogWarning($"Sound '{name}' not found!", this);
 
        return sound;
    }
}
 