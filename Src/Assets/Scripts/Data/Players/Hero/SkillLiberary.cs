using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SkillLibrary : MonoSingleton<SkillLibrary>
{
    // 存储所有技能实例
    public Dictionary<string, Skill> skillDic = new Dictionary<string, Skill>();

    private void Awake()
    {
        // 游戏开始时自动扫描并注册技能
        RegisterAllSkills();
    }

    /// <summary>
    /// 自动扫描并注册所有继承自 Skill 的类
    /// </summary>
    private void RegisterAllSkills()
    {
        skillDic.Clear();

        // 获取当前程序集里的所有类型
        var skillTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Skill).IsAssignableFrom(t));

        foreach (var type in skillTypes)
        {
            try
            {
                // 创建技能实例
                Skill skillInstance = Activator.CreateInstance(type) as Skill;

                if (skillInstance != null && !string.IsNullOrEmpty(skillInstance.ID))
                {
                    skillDic[skillInstance.ID] = skillInstance;
                    Debug.Log($"[SkillLibrary] 已注册技能: {skillInstance.ID} ({type.Name})");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SkillLibrary] 注册技能失败: {type.Name}, 错误: {e.Message}");
            }
        }

        Debug.Log($"[SkillLibrary] 总共注册了 {skillDic.Count} 个技能");
    }

    /// <summary>
    /// 根据技能ID获取技能实例
    /// </summary>
    public Skill GetSkill(string skillID)
    {
        if (skillDic.TryGetValue(skillID, out Skill skill))
            return skill;
        Debug.LogWarning($"[SkillLibrary] 找不到技能ID: {skillID}");
        return null;
    }
}