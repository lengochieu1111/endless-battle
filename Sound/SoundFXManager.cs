using Pattern.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : Singleton<SoundFXManager>
{
    public static void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        Transform audioSourceTransform = SoundFXSpawner.Instance.Spawn(SoundFXSpawner.SoundFXObject, spawnTransform.position, Quaternion.identity);
        audioSourceTransform.gameObject.SetActive(true);

        // Setup
        SoundFXObject soundFXObject = audioSourceTransform.GetComponent<SoundFXObject>();
        soundFXObject.ActivateSelf(audioClip, volume);
    }

}
