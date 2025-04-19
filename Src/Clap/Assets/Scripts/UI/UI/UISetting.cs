using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UISetting : UIWindow
{

    // 音量调节滑动条
    public Slider volumeSlider;

    // 退回主菜单按钮
    public Button backToMainMenuButton;
    private void Awake()
    {
        if (UIManager.Instance != null & UIManager.Instance.currentScene == 0)
        {
            this.backToMainMenuButton.gameObject.SetActive(false);
        }
    }

    // 音量调节方法
    public void OnVolumeChanged(float volume)
    {
        // 设置全局音量
        AudioListener.volume = volume;
        Debug.Log($"音量已调节为: {volume}");
    }

    // 退回主菜单方法
    public void OnBackToMainMenuClicked()
    {
        // 加载主菜单场景
        SceneManager.Instance.LoadScene("LoadingSence");
        UIManager.Instance.currentScene = UIManager.Scene.LoadingSence;
        Debug.Log("已退回主菜单");
    }
}