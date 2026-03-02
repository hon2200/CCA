using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Holds the raw skill data loaded from JSON files (MonsterSkill.json, HeroSkill.json).
/// Use SkillDefineData for deserialization since SkillDefine is abstract.
/// </summary>
public class SkillDatabaseOrigin : MonoSingleton<SkillDatabaseOrigin>
{
    public Dictionary<string, SkillDefineOrigin> OriginalMonsterSkillDic { get; private set; }
    public Dictionary<string, SkillDefineOrigin> OriginalHeroSkillDic { get; private set; }

    /// <summary>
    /// Loads MonsterSkill.json and HeroSkill.json into OriginalMonsterSkillDic and OriginalHeroSkillDic.
    /// </summary>
    public void LoadingSkills()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Skill&Buff/MonsterSkill.json");
        OriginalMonsterSkillDic = JsonLoader.DeserializeObject<Dictionary<string, SkillDefineOrigin>>(path);
        MyLog.PrintLoadedDictionary(OriginalMonsterSkillDic, "Log/Loading/MonsterSkills.txt");

        path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Skill&Buff/HeroSkill.json");
        OriginalHeroSkillDic = JsonLoader.DeserializeObject<Dictionary<string, SkillDefineOrigin>>(path);
        MyLog.PrintLoadedDictionary(OriginalHeroSkillDic, "Log/Loading/HeroSkills.txt");
    }
}
