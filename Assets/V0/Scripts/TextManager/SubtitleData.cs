using UnityEngine;

[System.Serializable]
public class SubtitleData
{
    public AudioClip clip;
    [TextArea] public string subtitle;
}