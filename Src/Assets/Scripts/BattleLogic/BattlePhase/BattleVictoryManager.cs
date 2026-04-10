using UnityEngine;

public class BattleVictoryManager : MonoBehaviour
{
    [Header("胜利结算UI")]
    [Tooltip("战斗胜利结算界面的根物体（需提前赋值，设为非激活状态）")]
    public GameObject victoryUI; // 赋值为你的"战斗胜利"UI对象

    [Header("胜利音效")]
    [Tooltip("胜利对应的音频剪辑")]
    public AudioClip victorySound;
    [Tooltip("音频播放音量（0~1）")]
    public float volume = 1f;

    // 私有组件
    private AudioSource audioSource;

    void Start()
    {
        // 初始化：确保结算界面初始隐藏
        if (victoryUI != null)
        {
            victoryUI.SetActive(false);
        }

        // 添加AudioSource组件用于播放音效
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = victorySound;
        audioSource.volume = volume;
        audioSource.playOnAwake = false; // 取消开机自动播放
    }

    // 对外暴露的胜利触发方法（供外部调用，如战斗逻辑结束时）
    public void TriggerVictory()
    {
        // 1. 播放胜利音效
        if (victorySound != null && audioSource != null)
        {
            audioSource.Play();
        }

        // 2. 激活胜利结算界面
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
        }
    }

    // 示例：通过碰撞触发胜利（可根据你的战斗逻辑替换为其他触发方式）
}
