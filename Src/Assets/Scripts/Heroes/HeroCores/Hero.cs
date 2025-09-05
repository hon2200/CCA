using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Hero
{
    public string ID { get; set; }

    public List<Skill> skills;
    public Hero(HeroDefine heroDefine)
    {
        skills = new List<Skill>();
        //添加英雄技能
        foreach (var hero in HeroDataBase.Instance.HeroDictionary)
        {
            if (hero.Key == heroDefine.ID)
                foreach (var skillID in hero.Value.SkillIDList)
                {
                    SkillLiberary.Instance.skillDic.TryGetValue(skillID, out var skill);
                    skills.Add(skill);
                }
        }
    }
}
