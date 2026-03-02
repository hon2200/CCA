using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SkillDatabase : MonoSingleton<SkillDatabase>
{
    // 存储所有技能实例
    public Dictionary<string, SkillDefine> skillDic = new Dictionary<string, SkillDefine>();

    /// <summary>Skills loaded from MonsterSkill.json.</summary>
    public Dictionary<string, MonsterSkill> monsterSkillDic = new Dictionary<string, MonsterSkill>();

    /// <summary>Skills loaded from HeroSkill.json.</summary>
    public Dictionary<string, HeroSkill> heroSkillDic = new Dictionary<string, HeroSkill>();

    /// <summary>
    /// Load JSON into SkillDatabaseOrigin, then register all concrete skills.
    /// </summary>
    public void LoadingSkills()
    {
        SkillDatabaseOrigin.Instance.LoadingSkills();
    }

    private void Awake()
    {
        // 游戏开始时自动扫描并注册技能
        LoadingSkills();
        RegisterAllSkills();
    }

    /// <summary>
    /// 自动扫描并注册所有继承自 Skill 的类；技能数据由各类型的 Init() 从 JSON 字典填入。
    /// 将怪物技能与英雄技能分别放入 monsterSkillDic 与 heroSkillDic。
    /// </summary>
    private void RegisterAllSkills()
    {
        skillDic.Clear();
        monsterSkillDic.Clear();
        heroSkillDic.Clear();

        // 获取当前程序集里的所有类型（排除抽象基类 MonsterSkill / HeroSkill，它们无参构造）
        var skillTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(SkillDefine).IsAssignableFrom(t)
                && t != typeof(MonsterSkill) && t != typeof(HeroSkill));

        foreach (var type in skillTypes)
        {
            try
            {
                SkillDefine skillInstance = Activator.CreateInstance(type) as SkillDefine;

                if (skillInstance != null && !string.IsNullOrEmpty(skillInstance.ID))
                {
                    skillDic[skillInstance.ID] = skillInstance;

                    if (skillInstance is MonsterSkill monsterSkill)
                        monsterSkillDic[monsterSkill.ID] = monsterSkill;
                    else if (skillInstance is HeroSkill heroSkill)
                        heroSkillDic[heroSkill.ID] = heroSkill;

                    Debug.Log($"[SkillDatabase] 已注册技能: {skillInstance.ID} ({type.Name})");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SkillDatabase] 注册技能失败: {type.Name}, 错误: {e.Message}");
            }
        }

        Debug.Log($"[SkillDatabase] 总共注册 {skillDic.Count} 个技能 (怪物: {monsterSkillDic.Count}, 英雄: {heroSkillDic.Count})");
    }

    /// <summary>
    /// 根据技能ID获取技能实例，没人用
    /// </summary>
    public SkillDefine GetSkill(string skillID)
    {
        if (skillDic.TryGetValue(skillID, out SkillDefine skill))
            return skill;
        Debug.LogWarning($"[SkillLibrary] 找不到技能ID: {skillID}");
        return null;
    }
}