using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Holds the raw skill data loaded from JSON files (MonsterSkill.json, HeroSkill.json).
/// Use SkillDefineData for deserialization since SkillDefine is abstract.
/// </summary>
public class SkillDatabaseOrigin : MonoSingleton<SkillDatabaseOrigin>
{
    public Dictionary<string, SkillDefineOrigin> OriginalEnemySkillDic { get; private set; }
    public Dictionary<string, SkillDefineOrigin> OriginalHeroSkillDic { get; private set; }

    /// <summary>
    /// Loads MonsterSkill.json and HeroSkill.json into OriginalMonsterSkillDic and OriginalHeroSkillDic.
    /// </summary>
    public void LoadingSkills()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero&Enemy/EnemySkill.json");
        OriginalEnemySkillDic = JsonLoader.DeserializeObject<Dictionary<string, SkillDefineOrigin>>(path);
        MyLog.PrintLoadedDictionary(OriginalEnemySkillDic, "Log/Loading/EnemySkills.txt");

        path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero&Enemy/HeroSkill.json");
        OriginalHeroSkillDic = JsonLoader.DeserializeObject<Dictionary<string, SkillDefineOrigin>>(path);
        MyLog.PrintLoadedDictionary(OriginalHeroSkillDic, "Log/Loading/HeroSkills.txt");
    }
}
