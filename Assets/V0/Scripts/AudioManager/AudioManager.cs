using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    public AudioSource AudioSource;

    
    public void LoadAudioClip(AudioClip clip)
    {
        AudioSource.PlayOneShot(clip);
    }
}