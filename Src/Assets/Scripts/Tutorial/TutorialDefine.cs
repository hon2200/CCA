using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TutorialDefine
{
    public string ID { get; set; }
    public string EnglishName { get; set; }
    public string Name { get; set; }
    public int Chapter { get; set; }
    public List<string> UnlockedSkillFinal { get; set; }
    public List<List<string>> EnemyList { get; set; }
    public List<List<string>> FriendList { get; set; }
    public List<string> DisabledAction { get; set; }
    public List<string> UnlockedAction { get; set; }
    public string HeroId { get; set; }
    public string NextLevel { get; set; }
    public string PreviousLevel { get; set; }

    public List<string> GetAllUnlockedActions()
    {
        List<string> availableActions = new();
        List<TutorialDefine> previousLevels = new();
        previousLevels.Add(this);
        availableActions.AddRange(UnlockedAction ?? new List<string>());
        while (previousLevels[previousLevels.Count - 1].PreviousLevel != null)
        {
            TutorialDatabase.Instance.TutorialDictionary.TryGetValue(previousLevels[previousLevels.Count - 1].PreviousLevel, out var anotherLevel);
            previousLevels.Add(anotherLevel);
            availableActions.AddRange(anotherLevel.UnlockedAction ?? new List<string>());
        }
        return availableActions;
    }

    public List<string> GetAllUnlockedSkills()
    {
        List<string> unlockedSkills = new();
        List<TutorialDefine> previousLevels = new();
        previousLevels.Add(this);
        unlockedSkills.AddRange(UnlockedSkillFinal ?? new List<string>());
        while (previousLevels[previousLevels.Count - 1].PreviousLevel != null)
        {
            TutorialDatabase.Instance.TutorialDictionary.TryGetValue(previousLevels[previousLevels.Count - 1].PreviousLevel, out var anotherLevel);
            previousLevels.Add(anotherLevel);
            unlockedSkills.AddRange(anotherLevel.UnlockedSkillFinal ?? new List<string>());
        }
        return unlockedSkills;
    }
}
