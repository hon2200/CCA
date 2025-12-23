﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class PlayerSkillManager : MonoSingleton<PlayerSkillManager>
{
    public GameObject SkillLoadingPanel;
    public Button AdvanceButton;
    
    public int skillSlots;
    public int skillSlots_change;
    public List<string> UnlockedSkills;
    
    // 选中的技能列表
    public List<string> SelectedSkills = new List<string>();
    
    // 在Inspector中拖拽赋值
    public TextMeshProUGUI remainingSlotsText;
    // 显示已选技能的文本
    public TextMeshProUGUI selectedSkillsText;

    private Transform contentRoot; // Content 节点
    public Sprite buttonBackgroundSprite;
    
    private Dictionary<string, GameObject> skillButtonCache = new Dictionary<string, GameObject>();
    // 新增：存储按钮的原始颜色，用于恢复
    private Dictionary<string, Color> buttonOriginalColors = new Dictionary<string, Color>();
    
    public void Start()
    {
        AdvanceButton.onClick.AddListener(ToBattle);
    
        // 确保面板是激活状态以便找到子物体
        bool wasActive = SkillLoadingPanel.activeSelf;
        if (!wasActive)
        {
            SkillLoadingPanel.SetActive(true);
        }
    
        // 找到 Content（SkillLoadingPanel 下的一个子物体）
        contentRoot = SkillLoadingPanel.transform.Find("Scroll/Content");
        
        // 如果Inspector中没有指定，尝试自动查找
        if (remainingSlotsText == null)
        {
            remainingSlotsText = SkillLoadingPanel.transform.Find("RemainingSlotsText")?.GetComponent<TextMeshProUGUI>();
            
            if (remainingSlotsText == null)
            {
                Debug.LogError("请将RemainingSlotsText拖拽到PlayerSkillManager脚本的remainingSlotsText字段中");
            }
        }
        
        // 尝试查找已选技能文本
        if (selectedSkillsText == null)
        {
            selectedSkillsText = SkillLoadingPanel.transform.Find("SelectedSkillsText")?.GetComponent<TextMeshProUGUI>();
        }
    
        // 恢复面板原来的状态
        if (!wasActive)
        {
            SkillLoadingPanel.SetActive(false);
        }
    
        // 检查是否找到Content
        if (contentRoot == null)
        {
            Debug.LogError("找不到 Content 节点！");
        }
    }

    private void Init()
    {
        LevelDefine currentLevel = LevelManager.Instance.GetCurrentLevel();
        skillSlots = currentLevel.PlayerSkillSlots;
        skillSlots_change = skillSlots;
        UnlockedSkills = currentLevel.GetAllUnlockedSkills();
        
        // 清空已选技能列表（新关卡开始时）
        ClearSelectedSkills();
        
        // 更新剩余技能槽显示
        UpdateRemainingSlotsDisplay();
        
        Debug.Log($"已解锁技能数量: {UnlockedSkills.Count}");
        Debug.Log($"技能槽数量: {skillSlots}, 当前剩余: {skillSlots_change}");
    }

    /// <summary>
    /// 清空已选技能列表
    /// </summary>
    private void ClearSelectedSkills()
    {
        SelectedSkills.Clear();
        UpdateSelectedSkillsDisplay();
        Debug.Log("已清空已选技能列表");
    }

    /// <summary>
    /// 更新剩余技能槽显示
    /// </summary>
    private void UpdateRemainingSlotsDisplay()
    {
        if (remainingSlotsText != null)
        {
            remainingSlotsText.text = $"RemainingSlots: {skillSlots_change}/{skillSlots}";
            UpdateTextColor();
        }
        else
        {
            Debug.LogWarning("remainingSlotsText未设置");
        }
    }

    /// <summary>
    /// 根据剩余槽位更新文本颜色
    /// </summary>
    private void UpdateTextColor()
    {
        if (skillSlots_change <= 0)
        {
            remainingSlotsText.color = Color.red;
        }
        else if (skillSlots_change <= skillSlots / 2)
        {
            remainingSlotsText.color = Color.yellow;
        }
        else
        {
            remainingSlotsText.color = Color.green;
        }
    }

    /// <summary>
    /// 更新已选技能显示 - 每个技能换行显示
    /// </summary>
    private void UpdateSelectedSkillsDisplay()
    {
        if (selectedSkillsText != null)
        {
            if (SelectedSkills.Count == 0)
            {
                // 如果没有技能，显示"None"
                selectedSkillsText.text = "CollectedSkills:\nNone";
            }
            else
            {
                // 使用StringBuilder构建带换行的文本
                StringBuilder sb = new StringBuilder();
                sb.Append("CollectedSkills:\n");
                
                // 每个技能占一行，可以添加序号
                for (int i = 0; i < SelectedSkills.Count; i++)
                {
                    // 格式: "1. 技能名称"
                    sb.AppendLine($"{i + 1}. {SelectedSkills[i]}");
                }
                
                selectedSkillsText.text = sb.ToString();
            }
        }
    }

    /// <summary>
    /// 添加已选技能到列表
    /// </summary>
    private void AddSelectedSkill(string skillName)
    {
        if (!SelectedSkills.Contains(skillName))
        {
            SelectedSkills.Add(skillName);
            UpdateSelectedSkillsDisplay();
            Debug.Log($"已添加技能到列表: {skillName}, 总数: {SelectedSkills.Count}");
        }
    }

    /// <summary>
    /// 从已选技能列表中移除技能
    /// </summary>
    private void RemoveSelectedSkill(string skillName)
    {
        if (SelectedSkills.Contains(skillName))
        {
            SelectedSkills.Remove(skillName);
            UpdateSelectedSkillsDisplay();
            Debug.Log($"已从列表移除技能: {skillName}, 剩余: {SelectedSkills.Count}");
        }
    }

    /// <summary>
    /// 设置技能按钮为选中状态（改变颜色）
    /// </summary>
    private void SetSkillButtonSelected(string skillName, bool isSelected)
    {
        if (skillButtonCache.TryGetValue(skillName, out GameObject btnObj))
        {
            Image img = btnObj.GetComponent<Image>();
            Button btn = btnObj.GetComponent<Button>();
            
            if (img != null && btn != null)
            {
                if (isSelected)
                {
                    // 保存原始颜色（如果是第一次）
                    if (!buttonOriginalColors.ContainsKey(skillName))
                    {
                        buttonOriginalColors[skillName] = img.color;
                    }
                    
                    // 设置为选中颜色（例如灰色）
                    img.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                    
                    // 也可以改变按钮的交互状态
                    btn.interactable = true; // 仍然可点击，以便取消选中
                }
                else
                {
                    // 恢复原始颜色
                    if (buttonOriginalColors.TryGetValue(skillName, out Color originalColor))
                    {
                        img.color = originalColor;
                    }
                    else
                    {
                        // 如果没有保存原始颜色，恢复默认颜色
                        img.color = Color.white;
                    }
                    
                    btn.interactable = true;
                }
            }
        }
    }

    /// <summary>
    /// 检查技能是否已被选中
    /// </summary>
    private bool IsSkillSelected(string skillName)
    {
        return SelectedSkills.Contains(skillName);
    }

    public void OpenSkillPanel()
    {
        Init();
        
        // 清除旧的按钮
        ClearExistingButtons();
        
        // 生成新的按钮
        GenerateSkillButtons();
        
        SkillLoadingPanel.SetActive(true);
    }

    /// <summary>
    /// 清除所有已存在的技能按钮
    /// </summary>
    private void ClearExistingButtons()
    {
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
        
        skillButtonCache.Clear();
        buttonOriginalColors.Clear(); // 同时清空颜色缓存
    }

    /// <summary>
    /// 动态生成技能按钮
    /// </summary>
    private void GenerateSkillButtons()
    {
        if (UnlockedSkills == null || UnlockedSkills.Count == 0)
        {
            Debug.LogWarning("没有可用的技能来生成按钮");
            return;
        }
        
        HashSet<string> processedSkills = new HashSet<string>();
        
        foreach (string skillName in UnlockedSkills)
        {
            if (processedSkills.Contains(skillName))
                continue;
            
            processedSkills.Add(skillName);
            
            // 创建按钮对象
            GameObject btnObj = new GameObject(skillName, typeof(RectTransform));
            btnObj.transform.SetParent(contentRoot, false);
            
            skillButtonCache[skillName] = btnObj;

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector3(3.71f, -2.44f, 0f);
            rt.sizeDelta = new Vector2(1.5054f, 0.99333f);
            rt.localScale = Vector3.one;
            
            // 添加Image组件
            Image img = btnObj.AddComponent<Image>();
            if (buttonBackgroundSprite != null)
            {
                img.sprite = buttonBackgroundSprite;
                img.type = Image.Type.Sliced;
            }
            else
            {
                img.color = Color.white; // 默认白色
            }
            img.raycastTarget = true;

            // 添加Button组件
            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            
            // 创建文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(btnObj.transform, false);

            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;

            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = skillName;
            tmpText.fontSize = 0.3f;
            tmpText.color = Color.black;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.raycastTarget = false;

            // 点击事件
            string currentSkillName = skillName;
            btn.onClick.AddListener(() => OnSkillButtonClicked(currentSkillName));
        }
    }

    /// <summary>
    /// 技能按钮点击事件处理 - 支持切换选中/取消选中
    /// </summary>
    private void OnSkillButtonClicked(string skillName)
    {
        if (IsSkillSelected(skillName))
        {
            // 如果已经选中，则取消选中
            OnDeselectSkill(skillName);
        }
        else
        {
            // 如果没有选中，则尝试选中
            OnSelectSkill(skillName);
        }
    }

    /// <summary>
    /// 选中技能
    /// </summary>
    private void OnSelectSkill(string skillName)
    {
        // 检查是否还有剩余技能槽
        if (skillSlots_change <= 0)
        {
            Debug.Log($"技能槽已用完，无法选择技能: {skillName}");
            return;
        }

        // 减少技能槽计数
        skillSlots_change--;
        UpdateRemainingSlotsDisplay();
        
        // 添加到已选技能列表
        AddSelectedSkill(skillName);
        
        // 设置按钮为选中状态
        SetSkillButtonSelected(skillName, true);
        
        Debug.Log($"已选择技能: {skillName}, 剩余技能槽: {skillSlots_change}");
    }

    /// <summary>
    /// 取消选中技能
    /// </summary>
    private void OnDeselectSkill(string skillName)
    {
        // 恢复技能槽计数
        skillSlots_change++;
        UpdateRemainingSlotsDisplay();
        
        // 从已选技能列表移除
        RemoveSelectedSkill(skillName);
        
        // 恢复按钮原始状态
        SetSkillButtonSelected(skillName, false);
        
        Debug.Log($"已取消选择技能: {skillName}, 剩余技能槽: {skillSlots_change}");
    }

    public void ToBattle()
    {
        // 可以将选中的技能列表传递给BattleManager
        if (SelectedSkills.Count > 0)
        {
            Debug.Log($"准备进入战斗，已选技能: {string.Join(", ", SelectedSkills)}");
            // 这里可以添加将技能列表传递给战斗系统的逻辑
        }
        
        SkillLoadingPanel.SetActive(false);
        BattleManager.Instance.OnStartGame("Level");
    }
    
    /// <summary>
    /// 重置技能选择状态
    /// </summary>
    public void ResetSkillSelection()
    {
        skillSlots_change = skillSlots;
        ClearSelectedSkills();
        UpdateRemainingSlotsDisplay();
        
        // 恢复所有按钮的原始状态
        foreach (var kvp in skillButtonCache)
        {
            SetSkillButtonSelected(kvp.Key, false);
        }
        
        Debug.Log("已重置所有技能选择状态");
    }
    
    /// <summary>
    /// 获取当前选中的技能列表
    /// </summary>
    public List<string> GetSelectedSkills()
    {
        return new List<string>(SelectedSkills);
    }
}