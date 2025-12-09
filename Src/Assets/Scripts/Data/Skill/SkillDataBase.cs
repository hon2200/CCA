using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillDataBase : MonoSingleton<SkillDataBase>
{
    public string path;
    public Dictionary<string, SkillDefine> MonsterSkillDic { get; set; }
    //读入所有玩家
    public void LoadingSkills()
    {
        path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Skill&Buff/MonsterSkill.json");
        MonsterSkillDic = JsonLoader.DeserializeObject<Dictionary<string, SkillDefine>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(MonsterSkillDic, "Log/Loading/MonsterSkills.txt");
    }
}
