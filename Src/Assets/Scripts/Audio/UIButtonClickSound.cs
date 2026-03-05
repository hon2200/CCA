using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClickSound : MonoBehaviour
{
    private Button _button;

    // 如果你需要给单个按钮单独设置音量，取消下面的注释
    // [Tooltip("单独设置这个按钮的音效音量（0.01-1）")]
    // [Range(0.01f, 1f)] public float buttonVolume = 1f;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(PlayClickSound);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("场景中没有AudioManager物体！");
            return;
        }

        // 仅播放音效，不修改任何音量参数
        AudioManager.Instance.PlayUIClickSound();

        // 如果需要单个按钮自定义音量，替换上面一行为：
        // AudioManager.Instance.SetUISoundVolume(buttonVolume);
        // AudioManager.Instance.PlayUIClickSound();
    }
}