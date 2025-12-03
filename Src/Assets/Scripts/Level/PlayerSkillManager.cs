using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerSkillManager : MonoSingleton<PlayerSkillManager>
{
    public GameObject SkillLoadingPanel;
    public Button AdvanceButton;
    
    public int skillSlots;
    public int skillSlots_change;
    public List<string> UnlockedSkills;

    private Transform contentRoot; // Content 节点
    public Sprite buttonBackgroundSprite; 
    public void Start()
    {
        AdvanceButton.onClick.AddListener(ToBattle);

        // 找到 Content（SkillLoadingPanel 下的一个子物体）
        contentRoot = SkillLoadingPanel.transform.Find("Content");
    }

    private void Init()
    {
        LevelDefine currentLevel = LevelManager.Instance.GetCurrentLevel();
        skillSlots = currentLevel.PlayerSkillSlots;
        skillSlots_change = skillSlots;
        UnlockedSkills = currentLevel.GetAllUnlockedSkills();
        UnlockedSkills =new List<string>
        {
            "Rapid Supply",
            "Candlelight Sanctuary",
            "Feedback"
        };
        Debug.Log($"初始化完成 - 技能槽数量: {skillSlots}, 当前剩余: {skillSlots_change}");
    }

    public void OpenSkillPanel()
    {
        Init();
        GenerateSkillButtons();
        SkillLoadingPanel.SetActive(true);
    }

    /// <summary>
    /// 动态生成技能按钮
    /// </summary>
    private void GenerateSkillButtons()
{
    foreach (string skillName in UnlockedSkills)
    {
        Debug.Log(skillName);

        // ------------ 创建按钮对象 ----------------
        GameObject btnObj = new GameObject(skillName, typeof(RectTransform));
        btnObj.transform.SetParent(contentRoot, false);
        btnObj.transform.SetAsLastSibling();

        RectTransform rt = btnObj.GetComponent<RectTransform>();

        // 设置Anchor和Pivot为middle center (0.5, 0.5)
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        // 设置位置和尺寸（根据参考按钮的数值）
        rt.anchoredPosition = new Vector3(3.71f, -2.44f, 0f);
        rt.sizeDelta = new Vector2(1.5054f, 0.99333f);

        // 设置旋转和缩放
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        
        // ------------ Image（背景） ----------------
        Image img = btnObj.AddComponent<Image>();
        // 使用Inspector中设置的背景图片
        if (buttonBackgroundSprite != null)
        {
            img.sprite = buttonBackgroundSprite;
            img.type = Image.Type.Sliced;
        }
        else
        {
            img.color = new Color(1f, 1f, 1f, 1f);
        }
        img.raycastTarget = true;

        // ------------ Button 组件 ----------------
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.interactable = true;
        Debug.Log("111");
        // 设置颜色过渡
        ColorBlock cb = new ColorBlock();
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(0.95f, 0.95f, 0.95f);
        cb.pressedColor = new Color(0.85f, 0.85f, 0.85f);
        cb.selectedColor = Color.white;
        cb.disabledColor = new Color(0.8f, 0.8f, 0.8f);
        cb.colorMultiplier = 1f;
        cb.fadeDuration = 0.1f;
        btn.colors = cb;

        // 设置导航模式
        Navigation navigation = new Navigation();
        navigation.mode = Navigation.Mode.Automatic;
        btn.navigation = navigation;

        // ------------ Text (TextMeshPro) ----------------
        GameObject textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(btnObj.transform, false);

        // 设置Text的RectTransform
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        textRT.pivot = new Vector2(0.5f, 0.5f);

        // 添加TextMeshPro组件
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = skillName;
        tmpText.fontSize = (float)0.3; // 字体大小设置为0.3
        tmpText.color = Color.black;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.raycastTarget = true; // 文本通常不需要接收射线检测

        // 设置标签和层
        btnObj.tag = "Untagged";
        btnObj.layer = LayerMask.NameToLayer("UI");
        
        // 设置Static标志（如果需要）
        // btnObj.isStatic = true;
        //点击
        string currentSkillName = skillName;
        btn.onClick.AddListener(() =>
        {
            
            OnSkillButtonClicked(currentSkillName);
            Debug.Log("点击事件成功添加");
        });

        Debug.Log("one button ok");
    }
    

}

    /// <summary>
    /// 技能按钮点击事件处理
    /// </summary>
    /// <param name="skillName">技能名称</param>
    private void OnSkillButtonClicked(string skillName)
    {
        // 检查是否还有剩余技能槽
        if (skillSlots_change <= 0)
        {
            Debug.Log($"技能槽已用完，无法选择技能: {skillName}");
            return;
        }

        // 减少技能槽计数
        skillSlots_change--;
        
        // 输出选择信息
        Debug.Log($"已选择技能: {skillName}");
        Debug.Log($"剩余技能槽: {skillSlots_change}");
        
        // 这里可以添加其他逻辑，比如：
        // - 禁用已选择的按钮
        // - 更新UI显示剩余技能槽
        // - 将选择的技能添加到已选择列表
    }

    public void ToBattle()
    {
        SkillLoadingPanel.SetActive(false);
        BattleManager.Instance.OnStartGame("Level");
    }
    
}
