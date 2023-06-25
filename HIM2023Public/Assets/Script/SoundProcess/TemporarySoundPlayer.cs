//TemporarySoundPlayer.cs

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class TemporarySoundPlayer : MonoBehaviour
{
    public AudioSource mAudioSource;

    public void Awake()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    public void SoundPlay(AudioClip clip, float minDistance, float maxDistance)
    {
        mAudioSource.clip = clip;
        mAudioSource.spatialBlend = 1.0f;
        mAudioSource.rolloffMode = AudioRolloffMode.Linear;
        mAudioSource.minDistance = minDistance;
        mAudioSource.maxDistance = maxDistance;
        StartCoroutine(COR_DestroyWhenFinish(mAudioSource.clip.length));
    }

    public void SoundPlay(AudioClip clip)
    {
        mAudioSource.clip = clip;
        mAudioSource.spatialBlend = 1.0f;
        mAudioSource.rolloffMode = AudioRolloffMode.Linear;
        mAudioSource.minDistance = 1;
        mAudioSource.maxDistance = 500;
        StartCoroutine(COR_DestroyWhenFinish(mAudioSource.clip.length));
    }

    private IEnumerator COR_DestroyWhenFinish(float clipLength)
    {
        mAudioSource.PlayOneShot(mAudioSource.clip);
        yield return new WaitForSeconds(clipLength);

        Destroy(gameObject);
    }
}