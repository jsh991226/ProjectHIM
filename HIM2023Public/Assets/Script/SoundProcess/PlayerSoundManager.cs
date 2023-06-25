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
    /// 오디오의 이름을 기반으로 찾는다.
    /// </summary>
    /// <param name="clipName">오디오의 이름(파일 이름 기준)</param>
    /// <returns></returns>
    public AudioClip GetClip(string clipName)
    {

        AudioClip clip = mClipsDictionary[clipName];
        if (clip == null) { Debug.LogError(clipName + "이 존재하지 않습니다.");  }

        return clip;
    }   

    /// <summary>
    /// 2D 사운드로 재생한다. 거리에 상관 없이 같은 소리 크기로 들린다.
    /// </summary>
    public void PlaySound2D(string clipName)
    {
        audioSource.PlayOneShot(GetClip(clipName));
    }


    /// <summary>
    /// 3D 사운드로 재생한다.
    /// </summary>
    public void PlaySound3D(string clipName, float minDistance = 0.0f, float maxDistance = 50.0f)
    {
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.PlayOneShot(GetClip(clipName));
    }
}
