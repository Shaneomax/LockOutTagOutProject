using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI subtitleText;
    public CanvasGroup subtitleGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Original method that can still be called
    public void PlayWithSubtitles(AudioSource source, List<SubtitleData> dataList)
    {
        if (source != null && dataList != null && dataList.Count > 0)
            StartCoroutine(PlayWithSubtitlesCoroutine(source, dataList)); // fixed
    }

    // Coroutine version that can be used to wait for completion
    public IEnumerator PlayWithSubtitlesCoroutine(AudioSource source, List<SubtitleData> dataList)
    {
        foreach (var data in dataList)
        {
            if (data.clip == null) continue;

            subtitleText.text = data.subtitle;
            StartCoroutine(FadeSubtitle(true));

            source.clip = data.clip;
            source.Play();

            yield return new WaitForSeconds(data.clip.length);

            StartCoroutine(FadeSubtitle(false));
            yield return new WaitForSeconds(0.2f);
        }

        subtitleText.text = "";
    }

    private IEnumerator FadeSubtitle(bool fadeIn)
    {
        float target = fadeIn ? 1 : 0;
        float duration = 0.3f;
        float start = subtitleGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            subtitleGroup.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        subtitleGroup.alpha = target;
    }
}