using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;

public class SkillLiberary : MonoSingleton<SkillLiberary>
{
    public Dictionary<string, Skill> skillDic = new Dictionary<string, Skill>();
    public void Start()
    {
        AddAllSkills();
    }
    public void AddSkill(Skill skill)
    {
        skillDic.Add(skill.ID, skill);
    }
    //我感觉这种方式来索引所有技能有点抽象了
    //每写一个新的技能都需要加一行代码
    public void AddAllSkills()
    {
        AddSkill(new MoutainCrusher());
        AddSkill(new LegionBreaker());
        AddSkill(new DaringBounty());
    }
}
