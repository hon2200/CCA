using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogSequenceFade : MonoBehaviour
{
    [Header("对话框列表（按顺序）")]
    public List<GameObject> dialogPanels;

    [Header("每个对话框对应的语音（顺序要对应）")]
    public List<AudioClip> dialogVoices;

    [Header("动画")]
    public float fadeInDuration = 0.4f;
    public float fadeOutDuration = 0.3f;
    public float startScale = 0.8f;
    public float finalScaleMultiplier = 1f;

    [Header("打字机效果")]
    public float typeSpeed = 0.05f;

    private int currentIndex = 0;
    private CanvasGroup currentCG;
    private RectTransform currentRT;
    private bool canClickNext;

    private TMP_Text tmpText;
    private string fullText;

    private AudioSource audioSource;
    private AudioSource voiceAudioSource;

    void Awake()
    {
        // 自动创建语音播放源
        voiceAudioSource = gameObject.AddComponent<AudioSource>();
        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.loop = false;

        foreach (var panel in dialogPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void Start()
    {
        ShowCurrentDialog();
    }

    void Update()
    {
        if (canClickNext && Input.GetMouseButtonDown(0))
        {
            canClickNext = false;
            PlayNextDialog();
        }
    }

    public void ShowCurrentDialog()
    {
        if (currentIndex >= dialogPanels.Count)
        {
            OnAllDialogFinished();
            return;
        }

        GameObject panel = dialogPanels[currentIndex];
        if (panel == null) return;

        currentCG = panel.GetComponent<CanvasGroup>();
        if (currentCG == null)
            currentCG = panel.AddComponent<CanvasGroup>();

        currentRT = panel.GetComponent<RectTransform>();

        // 找到文本
        tmpText = panel.GetComponentInChildren<TMP_Text>(true);
        if (tmpText != null)
        {
            fullText = tmpText.text;
            tmpText.text = "";
        }

        panel.SetActive(true);
        currentCG.alpha = 0;
        currentRT.localScale = Vector3.one * startScale;

        StartCoroutine(FadeIn(() =>
        {
            PlayCurrentVoice();

            if (tmpText != null)
                StartCoroutine(TypewriterEffect());
            else
                canClickNext = true;
        }));
    }

    public void PlayNextDialog()
    {
        // 切换时停止上一句语音
        StopCurrentVoice();

        StartCoroutine(FadeOut(() =>
        {
            currentIndex++;
            ShowCurrentDialog();
        }));
    }

    // 播放当前句语音
    void PlayCurrentVoice()
    {
        if (currentIndex < dialogVoices.Count && dialogVoices[currentIndex] != null)
        {
            voiceAudioSource.clip = dialogVoices[currentIndex];
            voiceAudioSource.Play();
        }
    }

    void StopCurrentVoice()
    {
        voiceAudioSource.Stop();
    }

    IEnumerator FadeIn(Action onComplete)
    {
        float t = 0;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            float rate = t / fadeInDuration;
            float ease = Mathf.Sin(rate * Mathf.PI * 0.5f);

            currentCG.alpha = Mathf.Lerp(0, 1, ease);
            currentRT.localScale = Vector3.Lerp(
                Vector3.one * startScale,
                Vector3.one * finalScaleMultiplier,
                ease);

            yield return null;
        }

        currentCG.alpha = 1;
        currentRT.localScale = Vector3.one * finalScaleMultiplier;
        onComplete?.Invoke();
    }

    IEnumerator FadeOut(Action onComplete)
    {
        float t = 0;
        float startAlpha = currentCG.alpha;
        Vector3 startScaleVec = currentRT.localScale;

        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float rate = t / fadeOutDuration;
            float ease = 1 - Mathf.Cos(rate * Mathf.PI * 0.5f);

            currentCG.alpha = Mathf.Lerp(startAlpha, 0, ease);
            currentRT.localScale = Vector3.Lerp(startScaleVec, Vector3.one * startScale, ease);

            yield return null;
        }

        currentCG.alpha = 0;
        currentRT.localScale = Vector3.one * startScale;
        currentCG.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    // 打字机效果
    IEnumerator TypewriterEffect()
    {
        canClickNext = false;
        tmpText.text = "";
        foreach (char c in fullText)
        {
            tmpText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        canClickNext = true;
    }

    void OnAllDialogFinished()
    {
        Debug.Log("全部对话框播放完成");
    }
}