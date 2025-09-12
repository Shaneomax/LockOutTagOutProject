using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayAudioSequentially(AudioSource source, List<AudioClip> clips)
    {
        if (source != null && clips != null && clips.Count > 0)
            StartCoroutine(PlaySequence(source, clips));
    }

    private IEnumerator PlaySequence(AudioSource source, List<AudioClip> clips)
    {
        foreach (var clip in clips)
        {
            if (clip == null) continue;

            source.clip = clip;
            source.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }

    public void PlayAudioListAtPoint(List<AudioClip> clips, Vector3 position)
    {
        if (clips == null) return;

        foreach (var clip in clips)
        {
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, position);
        }
    }
}