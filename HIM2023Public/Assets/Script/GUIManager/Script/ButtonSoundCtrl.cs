using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundCtrl : MonoBehaviour
{
    public AudioClip ClickSound;
    private AudioSource guiAudio;
    
    void Start()
    {
        guiAudio = GameObject.Find("GUIManager").GetComponent<AudioSource>();
    }
    public void PlaySound()
    {
        if (ClickSound != null)
        {
            Debug.Log("PLAY");
            guiAudio.clip = ClickSound;
            guiAudio.Play();    
        }
    }
}
