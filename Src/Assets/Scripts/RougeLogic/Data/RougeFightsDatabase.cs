using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RougeFightsDatabase : MonoSingleton<RougeFightsDatabase>
{
    public Dictionary<string, RougeFightDefine> FightDictionary { get; private set; }
    public RougeFightDefine CurrentFight { get; private set; }
    public string LastFightID { get; private set; }

    /// <summary>
    /// Load RougeFights.json into FightDictionary.
    /// Pattern is intentionally aligned with RelicDatabaseOrigin.LoadingRelics().
    /// </summary>
    public void LoadingFights()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/RougeMap/RougeFights.json");
        FightDictionary = JsonLoader.DeserializeObject<Dictionary<string, RougeFightDefine>>(path);
        MyLog.PrintLoadedDictionary(FightDictionary, "Log/Loading/RougeFights.txt");
    }

    /// <summary>
    /// Pick one random fight by room type ("Minion"/"Elite"/"Boss"), avoiding the immediately previous fight ID when possible.
    /// Stores result into <see cref="CurrentFight"/> and updates <see cref="LastFightID"/>.
    /// </summary>
    public RougeFightDefine PickRandomFightByType(string fightType)
    {
        if (FightDictionary == null || FightDictionary.Count == 0)
            LoadingFights();

        if (FightDictionary == null || FightDictionary.Count == 0 || string.IsNullOrEmpty(fightType))
            return null;

        var candidates = new List<RougeFightDefine>();
        foreach (var fight in FightDictionary.Values)
        {
            if (fight == null || string.IsNullOrEmpty(fight.Type))
                continue;
            if (!string.Equals(fight.Type, fightType, System.StringComparison.OrdinalIgnoreCase))
                continue;
            candidates.Add(fight);
        }

        if (candidates.Count == 0)
            return null;

        var filtered = new List<RougeFightDefine>();
        foreach (var fight in candidates)
        {
            if (!string.IsNullOrEmpty(LastFightID) && fight.ID == LastFightID)
                continue;
            filtered.Add(fight);
        }

        var pool = filtered.Count > 0 ? filtered : candidates;
        int index = Random.Range(0, pool.Count);
        CurrentFight = pool[index];
        LastFightID = CurrentFight?.ID;
        return CurrentFight;
    }
}
