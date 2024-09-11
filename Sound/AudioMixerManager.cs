using Pattern.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : Singleton<AudioMixerManager>
{
    [SerializeField] private AudioMixer _audioMixer;

    protected override void Start()
    {
        base.Start();

        SetttingsUI.Instance.MasterSlider.onValueChanged.AddListener(this.SetMasterVolume);
        SetttingsUI.Instance.SoundFXSlider.onValueChanged.AddListener(this.SetSoundFXVolume);
        SetttingsUI.Instance.MusicSlider.onValueChanged.AddListener(this.SetMusicVolume);
    }

    private void SetMasterVolume(float level)
    {
        this._audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }

    private void SetSoundFXVolume(float level)
    {
        this._audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);
    }

    private void SetMusicVolume(float level)
    {
        this._audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }

}
