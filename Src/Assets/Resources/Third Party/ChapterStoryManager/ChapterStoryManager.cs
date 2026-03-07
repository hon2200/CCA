using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class ChapterStoryManager : MonoBehaviour
{
    [Header("UI组件")]
    public GameObject storyPanel; // 黑屏背景Panel
    public TextMeshProUGUI storyText; // 显示剧情的文本组件
    public GameObject mainMenuObject; // 主菜单

    [Header("视频组件")]
    public RawImage videoScreen; // 用来显示视频的RawImage
    public VideoPlayer videoPlayer; // 视频播放器组件

    [Header("设置")]
    public float typeSpeed = 0.05f; // 打字速度

    [Header("剧情文案(对应章节1,2,3)")]
    [TextArea(3, 10)]
    public string[] chapterStories; // 记得在Inspector把Size设为3

    private string currentFullStory;
    private bool isTyping = false;
    private bool isStoryFinished = false;
    private Coroutine typingCoroutine;
    private bool isVideoPlaying = false; // 新增：跟踪视频播放状态

    // 绑定到按钮的方法
    public void SelectChapter(int chapterIndex)
    {
        if (chapterIndex >= chapterStories.Length) return;

        // 1. 打开黑屏界面，隐藏主菜单
        storyPanel.SetActive(true);
        if (mainMenuObject != null)
            mainMenuObject.SetActive(false); // 隐藏菜单，防止穿帮

        storyText.text = ""; // 清空文本

        // 判断是否是第1章(索引为0)
        if (chapterIndex == 0)
        {
            // 如果是第1章,启动视频流程
            StartCoroutine(PlayVideoThenStartStory(chapterIndex));
        }
        else
        {
            // 其他章节，直接开始打字机流程
            StartStoryLogic(chapterIndex);
        }
    }

    // 视频播放流程协程
    IEnumerator PlayVideoThenStartStory(int chapterIndex)
    {
        isVideoPlaying = true;

        // 显示视频层,隐藏文本层(暂时)
        videoScreen.gameObject.SetActive(true);
        storyText.gameObject.SetActive(false);

        // 准备并播放视频
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();

        // 等待视频播放结束
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // 视频播完后
        videoScreen.gameObject.SetActive(false); // 隐藏视频
        storyText.gameObject.SetActive(true); // 显示文本组件
        isVideoPlaying = false;

        // 开始原本的打字机流程
        StartStoryLogic(chapterIndex);
    }

    // 将原本的开始逻辑提取出来,方便复用
    void StartStoryLogic(int chapterIndex)
    {
        currentFullStory = chapterStories[chapterIndex];
        isStoryFinished = false;
        isTyping = true;
        typingCoroutine = StartCoroutine(TypewriterEffect());
    }

    IEnumerator TypewriterEffect()
    {
        storyText.text = ""; // 清空文本
        foreach (char c in currentFullStory)
        {
            storyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        isStoryFinished = true;
    }

    public void OnScreenClick()
    {
        // 如果视频正在播放，可以添加跳过视频的逻辑（可选）
        if (isVideoPlaying)
        {
            // 跳过视频：停止视频播放，直接进入打字流程
            videoPlayer.Stop();
            videoScreen.gameObject.SetActive(false);
            storyText.gameObject.SetActive(true);
            isVideoPlaying = false;

            // 找到当前章节索引（这里需要根据实际情况调整）
            int currentChapter = 0; // 默认第一章
            StartStoryLogic(currentChapter);
            return;
        }

        if (isTyping)
        {
            // 如果还在打字->瞬间显示完
            StopCoroutine(typingCoroutine);
            storyText.text = currentFullStory;
            isTyping = false;
            isStoryFinished = true;
        }
        else if (isStoryFinished)
        {
            // 如果字打完了->跳转场景
            EnterGameScene();
        }
    }

    void EnterGameScene()
    {
        Debug.Log("正在跳转到Show场景...");
        // 确保你的场景名字叫"Show"，大小写要完全一致
        SceneManager.LoadScene("Show");
    }
}