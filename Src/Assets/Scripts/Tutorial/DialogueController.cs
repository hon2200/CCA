using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

// 对话数据结构，可在Inspector配置多段对话
[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)] public string dialogueText; // 对话内容
    public AudioClip dialogueAudio; // 对应语音
    public float typeSpeed = 0.05f; // 打字速度
}

public class DialogueController : MonoBehaviour
{
    [Header("UI 引用")]
    [Tooltip("对话框父物体，用于显示/隐藏")]
    public GameObject dialoguePanel;
    [Tooltip("对话文字组件（Text或TextMeshProUGUI）")]
    public TextMeshProUGUI dialogueText;
    [Tooltip("角色头像框（可选，用于高亮）")]
    public Image characterPortrait;

    [Header("对话配置")]
    public DialogueLine[] dialogueLines; // 多段对话配置
    public bool autoPlayOnStart = true; // 场景加载自动播放
    public bool loopAudio = false; // 是否循环播放（对话类一般设为false）

    [Header("打字机效果")]
    public bool useTypewriterEffect = true; // 开启逐字显示
    public float defaultTypeSpeed = 0.05f; // 默认打字速度

    [Header("音频管理")]
    public AudioSource dialogueAudioSource; // 对话专用音频源（必须赋值）
    [Range(0f, 1f)] public float dialogueVolume = 1f; // 对话音量独立控制

    private int currentLineIndex = 0;
    private Coroutine typewriterCoroutine;
    private bool isDialoguePlaying = false;

    void Start()
    {
        // 初始化：隐藏对话框
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // 初始化对话音频源
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.volume = dialogueVolume;
            dialogueAudioSource.playOnAwake = false;
        }
        else
        {
            Debug.LogError("请给DialogueController赋值Dialogue Audio Source！");
        }

        // 自动播放对话
        if (autoPlayOnStart && dialogueLines != null && dialogueLines.Length > 0)
        {
            StartDialogue();
        }
    }

    /// <summary>
    /// 开始对话流程
    /// </summary>
    public void StartDialogue()
    {
        if (isDialoguePlaying) return;

        isDialoguePlaying = true;
        currentLineIndex = 0;
        // 显示对话框
        dialoguePanel.SetActive(true);
        // 播放第一段对话
        PlayDialogueLine(currentLineIndex);
    }

    /// <summary>
    /// 播放单段对话
    /// </summary>
    void PlayDialogueLine(int lineIndex)
    {
        if (lineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines[lineIndex];
        // 停止之前的打字机效果
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        // 停止之前的音频
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.Stop();
        }

        // 播放对应音频（完全独立，不依赖AudioManager）
        if (line.dialogueAudio != null && dialogueAudioSource != null)
        {
            dialogueAudioSource.clip = line.dialogueAudio;
            dialogueAudioSource.loop = loopAudio;
            dialogueAudioSource.volume = dialogueVolume;
            dialogueAudioSource.Play();
        }

        // 打字机效果显示文字
        if (useTypewriterEffect)
        {
            typewriterCoroutine = StartCoroutine(TypewriterText(line.dialogueText, line.typeSpeed));
        }
        else
        {
            dialogueText.text = line.dialogueText;
        }
    }

    /// <summary>
    /// 打字机效果协程
    /// </summary>
    IEnumerator TypewriterText(string text, float typeSpeed)
    {
        dialogueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // 文字显示完成后，等待音频结束自动进入下一段
        if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
        {
            yield return new WaitWhile(() => dialogueAudioSource.isPlaying);
            NextLine();
        }
        else
        {
            // 无音频时，等待1秒后自动下一段
            yield return new WaitForSeconds(1f);
            NextLine();
        }
    }

    /// <summary>
    /// 下一段对话
    /// </summary>
    public void NextLine()
    {
        if (!isDialoguePlaying) return;

        currentLineIndex++;
        PlayDialogueLine(currentLineIndex);
    }

    /// <summary>
    /// 结束对话，隐藏弹窗
    /// </summary>
    public void EndDialogue()
    {
        isDialoguePlaying = false;
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.Stop();
        }
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    /// <summary>
    /// 跳过当前对话，直接显示完整文字
    /// </summary>
    public void SkipCurrentLine()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            dialogueText.text = dialogueLines[currentLineIndex].dialogueText;
        }
    }

    // 可选：点击屏幕跳过/下一段
    void Update()
    {
        // 点击鼠标左键/屏幕，下一段对话
        if (Input.GetMouseButtonDown(0) && isDialoguePlaying)
        {
            // 如果正在打字，直接显示完整文字；否则下一段
            if (typewriterCoroutine != null)
            {
                SkipCurrentLine();
            }
            else
            {
                NextLine();
            }
        }
    }

    /// <summary>
    /// 动态修改对话音量（可在Inspector实时调整）
    /// </summary>
    public void SetDialogueVolume(float volume)
    {
        dialogueVolume = Mathf.Clamp01(volume);
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.volume = dialogueVolume;
        }
    }
}