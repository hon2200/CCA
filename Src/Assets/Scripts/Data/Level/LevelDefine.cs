using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LevelDefine
{
    public string ID { get; set; }
    public string EnglishName { get; set; }
    public string Name { get; set; }
    public int Chapter { get; set; }
    public List<string> UnlockedSkillFinal { get; set; }
    public int PlayerSkillSlots { get; set; }
    public List<List<string>> EnemyList { get; set; }
    public List<List<string>> FriendList { get; set; }
    public List<string> UnlockedAction { get; set; }
    public int PlayerHP { get; set; }
    public List<int> PlayerInitialResource { get; set; }
    public string NextLevel { get; set; }
    public string PreviousLevel { get; set; }

    public List<string> GetAllUnlockedActions()
    {
        List<string> availableActions = new();
        List<LevelDefine> previousLevels = new();
        previousLevels.Add(this);
        availableActions.AddRange(UnlockedAction);
        while (previousLevels[previousLevels.Count - 1].PreviousLevel != null)
        {
            LevelDataBase.Instance.LevelDictionary.TryGetValue(previousLevels[previousLevels.Count - 1].PreviousLevel, out var anotherLevel);
            previousLevels.Add(anotherLevel);
            availableActions.AddRange(anotherLevel.UnlockedAction);
        }
        return availableActions;
    }

    public List<string> GetAllUnlockedSkills()
    {
        List<string> unlockedSkills = new();
        List<LevelDefine> previousLevels = new();
        previousLevels.Add(this);
        while (previousLevels[previousLevels.Count - 1].PreviousLevel != null)
        {
            LevelDataBase.Instance.LevelDictionary.TryGetValue(previousLevels[previousLevels.Count - 1].PreviousLevel, out var anotherLevel);
            previousLevels.Add(anotherLevel);
            unlockedSkills.AddRange(anotherLevel.UnlockedSkillFinal);
        }
        return unlockedSkills;
    }

    public int GetIncreasedSkillSlots()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(PreviousLevel, out var previousLevel);
        return -previousLevel.PlayerSkillSlots + PlayerSkillSlots;
    }
    public int GetIncreasedMaxHP()
    {
        LevelDataBase.Instance.LevelDictionary.TryGetValue(PreviousLevel, out var previousLevel);
        return -previousLevel.PlayerHP + PlayerHP;
    }
}