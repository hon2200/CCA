using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections; 


/// <summary>
/// 轻量化面板动画切换器
/// 核心功能：点击按钮切换面板，实现淡入淡出动画
/// </summary>
public class PanelAnimationSwitcher : MonoBehaviour
{
    // 面板配置（可在Inspector绑定多个面板）
    [System.Serializable]
    public class PanelData
    {
        public string panelName;       // 面板名称（用于索引）
        public GameObject panelObject; // 面板物体
        public Button switchButton;    // 触发该面板显示的按钮
    }

    [Header("核心配置")]
    public PanelData[] panels;                // 所有面板数据
    public int defaultPanelIndex = 0;         // 默认显示的面板索引
    [Range(0.75f, 4)] public float animSpeed = 1f;         // 动画播放速度
    [Range(0, 1)] public float animSmoothness = 0.25f;     // 动画过渡平滑度
    [Range(0.75f, 4)] public float disablePanelDelay = 1f; // 淡出后延迟禁用面板的时间

    // 动画状态名称（需和Animator控制器中的状态名一致）
    [Header("动画状态配置")]
    public string panelFadeInState = "Main Panel In";
    public string panelFadeOutState = "Main Panel Out";

    private GameObject currentPanel;  // 当前显示的面板
    private Coroutine disableCoroutine; // 延迟禁用面板的协程

    void Start()
    {
        // 初始化：绑定按钮事件 + 显示默认面板
        InitButtonEvents();
        ShowDefaultPanel();
    }

    /// <summary>
    /// 初始化所有按钮的点击事件
    /// </summary>
    void InitButtonEvents()
    {
        if (panels == null || panels.Length == 0)
        {
            Debug.LogError("未配置任何面板数据！");
            return;
        }

        foreach (var panelData in panels)
        {
            if (panelData.switchButton != null)
            {
                // 绑定按钮点击事件（传当前面板数据）
                panelData.switchButton.onClick.AddListener(() =>
                    SwitchPanel(panelData.panelName));
            }
            else
            {
                Debug.LogWarning($"面板[{panelData.panelName}]未绑定切换按钮！");
            }
        }
    }

    /// <summary>
    /// 显示默认面板（无淡出，直接淡入）
    /// </summary>
    void ShowDefaultPanel()
    {
        // 校验默认索引有效性
        if (defaultPanelIndex < 0 || defaultPanelIndex >= panels.Length)
        {
            Debug.LogError("默认面板索引无效，已重置为0！");
            defaultPanelIndex = 0;
        }

        // 禁用所有面板
        foreach (var panelData in panels)
        {
            if (panelData.panelObject != null)
                panelData.panelObject.SetActive(false);
        }

        // 显示默认面板并播放淡入动画
        var defaultPanel = panels[defaultPanelIndex];
        if (defaultPanel.panelObject != null)
        {
            currentPanel = defaultPanel.panelObject;
            currentPanel.SetActive(true);
            PlayPanelAnimation(currentPanel, panelFadeInState, 0); // 立即淡入
        }
    }

    /// <summary>
    /// 核心方法：切换到指定名称的面板（带动画）
    /// </summary>
    /// <param name="targetPanelName">目标面板名称</param>
    public void SwitchPanel(string targetPanelName)
    {
        // 查找目标面板
        PanelData targetPanelData = null;
        foreach (var panelData in panels)
        {
            if (panelData.panelName == targetPanelName)
            {
                targetPanelData = panelData;
                break;
            }
        }

        if (targetPanelData == null || targetPanelData.panelObject == null)
        {
            Debug.LogError($"未找到面板[{targetPanelName}]或面板物体为空！");
            return;
        }

        // 如果是当前面板，直接返回
        if (currentPanel == targetPanelData.panelObject) return;

        // 停止上一次的延迟禁用协程
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        // 1. 播放当前面板的淡出动画
        if (currentPanel != null)
        {
            PlayPanelAnimation(currentPanel, panelFadeOutState, animSmoothness);
            // 延迟禁用当前面板（等淡出动画完成）
            disableCoroutine = StartCoroutine(DisablePanelAfterDelay(currentPanel));
        }

        // 2. 显示目标面板并播放淡入动画
        targetPanelData.panelObject.SetActive(true);
        PlayPanelAnimation(targetPanelData.panelObject, panelFadeInState, animSmoothness);

        // 更新当前面板
        currentPanel = targetPanelData.panelObject;
    }

    /// <summary>
    /// 通用面板动画播放方法（核心复用逻辑）
    /// </summary>
    /// <param name="panelObj">目标面板</param>
    /// <param name="animState">动画状态名</param>
    /// <param name="transitionTime">过渡平滑时间</param>
    public void PlayPanelAnimation(GameObject panelObj, string animState, float transitionTime)
    {
        Animator animator = panelObj.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"面板[{panelObj.name}]未挂载Animator组件！");
            return;
        }

        // 设置动画速度
        animator.SetFloat("Anim Speed", animSpeed);

        // 播放动画（支持平滑过渡）
        if (transitionTime <= 0)
            animator.Play(animState);
        else
            animator.CrossFade(animState, transitionTime);
    }

    /// <summary>
    /// 延迟禁用面板（等待淡出动画完成）
    /// </summary>
    IEnumerator DisablePanelAfterDelay(GameObject panelToDisable)
    {
        yield return new WaitForSecondsRealtime(disablePanelDelay);
        panelToDisable.SetActive(false);
    }

    // 可选：通过索引切换面板（备用方法）
    public void SwitchPanelByIndex(int index)
    {
        if (index < 0 || index >= panels.Length)
        {
            Debug.LogError("面板索引无效！");
            return;
        }
        SwitchPanel(panels[index].panelName);
    }
}
