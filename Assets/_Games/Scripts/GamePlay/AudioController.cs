
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AudioController : Singleton<AudioController>
{
    [SerializeField] private AudioSource _bgMusic;

    public AudioSource bgMusic => _bgMusic;

    [SerializeField] private AudioClip[] _bgMusics;

    [SerializeField] private Sound[] _sounds;

    private Dictionary<SoundKind, Sound> _soundContainer;

    [Button]
    public void Validate()
    {
        List<SoundKind> soundKinds = new List<SoundKind>();
        bool valid = true;
        foreach (var item in _sounds)
        {
            if (item.clip == null)
            {
                Debug.LogError($"{item.soundKind} is null");
                valid = false;
            }

            if (item.num <= 0)
            {
                Debug.LogError($"Num {item.soundKind} is {item.num}");
                valid = false;
            }
            if (item.volume <= 0)
            {
                Debug.LogError($"Volume {item.soundKind} is {item.volume}");
                valid = false;
            }

            if (soundKinds.Contains(item.soundKind))
            {
                Debug.LogError($"{item.soundKind} was added more time");
                valid = false;
            }
            else
            {
                soundKinds.Add(item.soundKind);
            }
        }

        if (valid)
        {
            Debug.Log("Data is valid");
        }
    }


    [Button]
    public void GetIndex(SoundKind soundKind)
    {
        for (int i = 0; i < _sounds.Length; i++)
        {
            if (_sounds[i].soundKind == soundKind)
            {
                Debug.LogError($"Index: {i}");
                return;
            }
        }
        Debug.LogError($"Not found");
    }

    protected override void Awake()
    {
        base.Awake();

        _soundContainer = new Dictionary<SoundKind, Sound>();

        foreach (Sound s in _sounds)
        {
            s.Init(transform);
            _soundContainer.Add(s.soundKind, s);
        }
    }


    [Button]
    public void PlayMusic()
    {

    }

    [Button]
    public void StopMusic()
    {
        _bgMusic.Stop();
    }

    public static void SetVolumeMusic(float volume)
    {
        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }
        if (Instance.bgMusic == null)
        {
            return;
        }
        Instance.bgMusic.volume = volume;
    }

    [Button]
    public static void PlaySound(SoundKind kind)
    {
        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }

        if (!Instance._soundContainer.ContainsKey(kind))
        {
            LogUtils.LogError($"Sound {kind} not found !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Instance._soundContainer[kind].Play();
    }

    [Button]
    public static void PlaySound(SoundKind kind, float volume)
    {
        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }

        if (!Instance._soundContainer.ContainsKey(kind))
        {
            LogUtils.LogError($"Sound {kind} not found !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Instance._soundContainer[kind].Play(volume);
    }

    public static void Stop(SoundKind kind)
    {
        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }

        if (!Instance._soundContainer.ContainsKey(kind))
        {
            LogUtils.LogError($"Sound {kind} not found");
            return;
        }
        Instance._soundContainer[kind].Stop();
    }
}

public enum SoundKind
{
    Touch,
    Merge,
    Return
}

[System.Serializable]
public class Sound
{
    public SoundKind soundKind;
    public int num;
    [Range(0, 1)]
    public float volume = 1;

    public AudioClip clip;


    private AudioSource[] sources;

    public void Init(Transform parent)
    {
        if (clip == null)
        {
            LogUtils.LogError($"{soundKind} is null !!!!!!!!!!!!!!!!!");
            return;
        }
        sources = new AudioSource[num];
        for (int i = 0; i < num; i++)
        {
            var obj = new GameObject(clip.name);
            obj.transform.SetParent(parent);

            var source = obj.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            sources[i] = source;
        }
    }

    public void Play()
    {
        for (int i = 0; i < num; i++)
        {
            if (!sources[i].isPlaying)
            {
                sources[i].volume = volume;
                sources[i].Play();
                return;
            }
        }
    }

    public void Play(float volume)
    {
        for (int i = 0; i < num; i++)
        {
            if (!sources[i].isPlaying)
            {
                sources[i].volume = volume;
                sources[i].Play();
                return;
            }
        }
    }

    public void Stop()
    {
        for (int i = 0; i < num; i++)
        {
            if (sources[i].isPlaying)
            {
                sources[i].Stop();
            }
        }
    }


}