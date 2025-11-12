using System;
using System.Collections.Generic;

public class Hero
{
    public string ID { get; set; }
    public List<Skill> skills;

    public Hero(Player thisPlayer, HeroDefine heroDefine)
    {
        ID = heroDefine.ID;
        skills = new List<Skill>();

        // 添加英雄技能
        if (HeroDataBase.Instance.HeroDictionary.TryGetValue(heroDefine.ID, out var heroData))
        {
            foreach (var skillID in heroData.SkillIDList)
            {
                if (SkillLibrary.Instance.skillDic.TryGetValue(skillID, out var skill))
                {
                    skills.Add(skill);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"技能ID不存在: {skillID}");
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning($"英雄ID不存在: {heroDefine.ID}");
        }
    }

    public Hero(Player thisPlayer, string heroID, List<string> skillList)
    {
        ID = heroID;
        skills = new List<Skill>();

        if (skillList == null)
            return;
        // 添加英雄技能
        foreach (var skillID in skillList)
        {
            if (SkillLibrary.Instance.skillDic.TryGetValue(skillID, out var skill))
            {
                skills.Add(skill);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"技能ID不存在: {skillID}");
            }
        }
    }

}