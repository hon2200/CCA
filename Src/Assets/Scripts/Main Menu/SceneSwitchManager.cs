using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 轻量化场景切换管理器
/// 直接绑定按钮与场景跳转逻辑，无加载过渡
/// </summary>
public class SceneSwitcherManager : MonoBehaviour
{
    // 【可在Inspector面板绑定】跳转到Free Game场景的按钮
    [Header("场景跳转按钮绑定")]
    public Button freeGameJumpButton;
    public Button mapJumpButton;
    public Button MainMenuJumpButton;
    public Button TutorialJumpButton;


    // 【可配置】Free Game场景名称（确保和Build Settings中一致）
    [Header("场景名称配置")]
    public string freeGameSceneName = "Free Game";
    public string RougeMapSceneName = "RougeMap";
    public string MainMenuSceneName = "Main Menu (Desktop)";
    public string TutorialSceneName = "Tutorial";


    private void Awake()
    {
        // 初始化按钮点击事件
        InitButtonEvents();
    }

    /// <summary>
    /// 初始化所有按钮的点击事件
    /// </summary>
    private void InitButtonEvents()
    {
        // 绑定Free Game跳转按钮的点击事件
        if (freeGameJumpButton != null)
        {
            // 先移除已有事件（避免重复绑定）
            freeGameJumpButton.onClick.RemoveAllListeners();
            // 添加跳转逻辑
            freeGameJumpButton.onClick.AddListener(OnFreeGameButtonClicked);
        }
        else
        {
            Debug.LogWarning("未绑定Free Game跳转按钮，请在Inspector面板赋值！");
        }

        // 绑定Rouge Map跳转按钮的点击事件
        if (mapJumpButton != null)
        {
            // 先移除已有事件（避免重复绑定）
            mapJumpButton.onClick.RemoveAllListeners();
            // 添加跳转逻辑
            mapJumpButton.onClick.AddListener(OnRougeMapButtonClicked);
        }
        else
        {
            Debug.LogWarning("未绑定RougeMap跳转按钮，请在Inspector面板赋值！");
        }

        // 绑定Main Menu跳转按钮的点击事件
        if (MainMenuJumpButton != null)
        {
            // 先移除已有事件（避免重复绑定）
            MainMenuJumpButton.onClick.RemoveAllListeners();
            // 添加跳转逻辑
            MainMenuJumpButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
        else
        {
            Debug.LogWarning("未绑定Main Menu跳转按钮，请在Inspector面板赋值！");
        }

        if (TutorialJumpButton != null)
        {
            // 先移除已有事件（避免重复绑定）
            TutorialJumpButton.onClick.RemoveAllListeners();
            // 添加跳转逻辑
            TutorialJumpButton.onClick.AddListener(OnTutorialButtonClicked);
        }
        else
        {
            Debug.LogWarning("未绑定Free Game跳转按钮，请在Inspector面板赋值！");
        }
    }

    /// <summary>
    /// Free Game按钮点击回调（核心跳转逻辑）
    /// </summary>
    public void OnFreeGameButtonClicked()
    {
        // 验证场景名称有效性
        if (string.IsNullOrEmpty(freeGameSceneName))
        {
            Debug.LogError("Free Game场景名称为空，请检查配置！");
            return;
        }

        // 验证场景是否在Build Settings中
        if (!IsSceneInBuildSettings(freeGameSceneName))
        {
            Debug.LogError($"场景「{freeGameSceneName}」未添加到Build Settings，请先添加！");
            return;
        }

        // 直接同步跳转场景（无加载过渡）
        SceneManager.LoadScene(freeGameSceneName);
    }

    public void OnTutorialButtonClicked()
    {
        // 验证场景名称有效性
        if (string.IsNullOrEmpty(TutorialSceneName))
        {
            Debug.LogError("Tutorial场景名称为空，请检查配置！");
            return;
        }

        // 验证场景是否在Build Settings中
        if (!IsSceneInBuildSettings(TutorialSceneName))
        {
            Debug.LogError($"场景「{TutorialSceneName}」未添加到Build Settings，请先添加！");
            return;
        }

        // 直接同步跳转场景（无加载过渡）
        SceneManager.LoadScene(TutorialSceneName);
    }

    public void OnRougeMapButtonClicked()
    {
        // 验证场景名称有效性
        if (string.IsNullOrEmpty(RougeMapSceneName))
        {
            Debug.LogError("RoughMap场景名称为空，请检查配置！");
            return;
        }

        // 验证场景是否在Build Settings中
        if (!IsSceneInBuildSettings(RougeMapSceneName))
        {
            Debug.LogError($"场景「{RougeMapSceneName}」未添加到Build Settings，请先添加！");
            return;
        }

        // 直接同步跳转场景（无加载过渡）
        SceneManager.LoadScene(RougeMapSceneName);
    }

    public void OnMainMenuButtonClicked()
    {
        // 验证场景名称有效性
        if (string.IsNullOrEmpty(MainMenuSceneName))
        {
            Debug.LogError("Main Menu场景名称为空，请检查配置！");
            return;
        }

        // 验证场景是否在Build Settings中
        if (!IsSceneInBuildSettings(MainMenuSceneName))
        {
            Debug.LogError($"场景「{MainMenuSceneName}」未添加到Build Settings，请先添加！");
            return;
        }

        // 直接同步跳转场景（无加载过渡）
        SceneManager.LoadScene(MainMenuSceneName);
    }
    /// <summary>
    /// 扩展：添加新场景跳转按钮时，复制以下模板修改即可
    /// 示例：新场景按钮绑定 + 跳转函数
    /// </summary>
    // [Header("新增场景按钮")]
    // public Button newSceneJumpButton;
    // public string newSceneName = "New Scene";
    // 
    // private void InitNewSceneButton()
    // {
    //     if (newSceneJumpButton != null)
    //     {
    //         newSceneJumpButton.onClick.RemoveAllListeners();
    //         newSceneJumpButton.onClick.AddListener(OnNewSceneButtonClicked);
    //     }
    // }
    // 
    // public void OnNewSceneButtonClicked()
    // {
    //     if (string.IsNullOrEmpty(newSceneName))
    //     {
    //         Debug.LogError("新场景名称为空！");
    //         return;
    //     }
    //     SceneManager.LoadScene(newSceneName);
    // }

    /// <summary>
    /// 辅助方法：检查场景是否在Build Settings中
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <returns>是否存在</returns>
    private bool IsSceneInBuildSettings(string sceneName)
    {
        // 遍历Build Settings中的所有场景
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            // 从路径中提取场景名称（去掉路径和后缀）
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(path);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 【可选】手动调用跳转（比如通过其他逻辑触发，而非按钮点击）
    /// </summary>
    public void JumpToFreeGameScene()
    {
        OnFreeGameButtonClicked();
    }

    public void JumpToRougeMap()
    {
        OnRougeMapButtonClicked();
    }

    public void JumpToMainMenu()
    {
        OnMainMenuButtonClicked();
    }
}