using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
public class PlayerSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    private Dictionary<string, AudioClip> mClipsDictionary;

    [SerializeField] private AudioClip[] mPreloadClips;

    [SerializeField] private AudioMixer mAudioMixer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mClipsDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in mPreloadClips)
        {
            mClipsDictionary.Add(clip.name, clip);
        }
    }


    /// <summary>
    /// ������� �̸��� ������� ã�´�.
    /// </summary>
    /// <param name="clipName">������� �̸�(���� �̸� ����)</param>
    /// <returns></returns>
    public AudioClip GetClip(string clipName)
    {

        AudioClip clip = mClipsDictionary[clipName];
        if (clip == null) { Debug.LogError(clipName + "�� �������� �ʽ��ϴ�.");  }

        return clip;
    }   

    /// <summary>
    /// 2D ����� ����Ѵ�. �Ÿ��� ��� ���� ���� �Ҹ� ũ��� �鸰��.
    /// </summary>
    public void PlaySound2D(string clipName)
    {
        audioSource.PlayOneShot(GetClip(clipName));
    }


    /// <summary>
    /// 3D ����� ����Ѵ�.
    /// </summary>
    public void PlaySound3D(string clipName, float minDistance = 0.0f, float maxDistance = 50.0f)
    {
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.PlayOneShot(GetClip(clipName));
    }
}
