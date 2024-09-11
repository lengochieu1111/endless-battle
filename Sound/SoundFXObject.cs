using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXObject : RyoMonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._audioSource == null)
        {
            this._audioSource = GetComponent<AudioSource>();
        }
    }

    public void ActivateSelf(AudioClip audioClip, float volume)
    {
        this._audioSource.clip = audioClip;
        this._audioSource.volume = volume;
        this._audioSource.Play();

        // Destroy
        float clipLength = audioClip.length;
        Invoke("DestroySelf", clipLength);
    }

    private void DestroySelf()
    {
        SoundFXSpawner.Instance.DestroyObjectPooling(this.transform);
    }


}
