using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIMouseHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [Header("悬浮音效设置")]
    [Tooltip("单独设置该物体的悬浮音量（-1则使用AudioManager全局音量）")]
    [Range(-1f, 1f)] public float customHoverVolume = -1f;
    [Tooltip("冷却时间（避免反复播放，单位：秒）")]
    public float coolDownTime = 0.5f;

    private float _coolDownTimer;

    private void Update()
    {
        if (_coolDownTimer > 0)
        {
            _coolDownTimer -= Time.deltaTime;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        if (AudioManager.Instance == null || _coolDownTimer > 0)
        {
            return;
        }

        _coolDownTimer = coolDownTime;

        if (customHoverVolume >= 0f && customHoverVolume <= 1f)
        {
            AudioManager.Instance.SetUIHoverVolume(customHoverVolume);
        }

        AudioManager.Instance.PlayUIHoverSound();
    }
}