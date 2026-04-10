using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecruitHero : EventDefine
{
    private readonly Dictionary<string, HeroDefine> recruitableByName = new Dictionary<string, HeroDefine>();

    public RecruitHero() : base("Tavern") { }

    protected override void Init()
    {
        base.Init();
        BuildRecruitOptions();
    }

    private void BuildRecruitOptions()
    {
        Options = new List<string>();
        recruitableByName.Clear();

        var heroDb = HeroDataBase.Instance?.HeroDictionary;
        var rougePlayer = RougeManager.Instance?.rougePlayer;
        if (heroDb == null || rougePlayer == null)
            return;

        var candidates = new List<HeroDefine>();
        foreach (var kv in heroDb)
        {
            HeroDefine hero = kv.Value;
            if (hero == null || string.IsNullOrEmpty(hero.ID) || string.IsNullOrEmpty(hero.Name))
                continue;
            if (rougePlayer.HasHero(hero.ID))
                continue;

            candidates.Add(hero);
        }

        // Randomly pick up to 3 unique heroes.
        int pickCount = Mathf.Min(3, candidates.Count);
        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            HeroDefine picked = candidates[index];
            candidates.RemoveAt(index);

            // Keep option labels unique even if two heroes share the same display name.
            string optionName = picked.Name;
            if (recruitableByName.ContainsKey(optionName))
                optionName = $"{picked.Name} ({picked.ID})";

            recruitableByName[optionName] = picked;
            Options.Add(optionName);
        }
    }

    public override void OnChoose(string option)
    {
        if (string.IsNullOrEmpty(option))
            return;

        if (!recruitableByName.TryGetValue(option, out var heroDefine) || heroDefine == null)
            return;

        RougeManager.Instance?.rougePlayer?.RecruitHero(heroDefine);
    }
}

public class ChooseRelic : EventDefine
{
    private readonly Dictionary<string, RelicDefine> relicByOption = new Dictionary<string, RelicDefine>();

    public ChooseRelic() : base("SacredCemetery") { }

    protected override void Init()
    {
        base.Init();
        BuildRelicOptions();
    }

    private void BuildRelicOptions()
    {
        Options = new List<string>();
        relicByOption.Clear();

        var relicDb = RelicDatabase.Instance?.RelicDictionary;
        var rougePlayer = RougeManager.Instance?.rougePlayer;
        if (relicDb == null || rougePlayer == null)
            return;

        var candidates = new List<RelicDefine>();
        foreach (var kv in relicDb)
        {
            RelicDefine relic = kv.Value;
            if (relic == null || string.IsNullOrEmpty(relic.ID) || string.IsNullOrEmpty(relic.Name))
                continue;
            if (rougePlayer.HasRelic(relic.ID))
                continue;

            candidates.Add(relic);
        }

        Debug.Log(candidates.Count);

        int pickCount = Mathf.Min(3, candidates.Count);
        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            RelicDefine picked = candidates[index];
            candidates.RemoveAt(index);

            string optionName = picked.Name;
            if (relicByOption.ContainsKey(optionName))
                optionName = $"{picked.Name} ({picked.ID})";

            relicByOption[optionName] = picked;
            Options.Add(optionName);
        }
    }

    public override void OnChoose(string option)
    {
        if (string.IsNullOrEmpty(option))
            return;
        if (!relicByOption.TryGetValue(option, out var relic) || relic == null)
            return;

        RougeManager.Instance?.rougePlayer?.GetRelic(relic, allowDuplicate: false);
    }
}

public class ChooseCard : EventDefine
{
    private readonly Dictionary<string, ActionDefine> cardByOption = new Dictionary<string, ActionDefine>();

    public ChooseCard() : base("SoulFountain") { }

    protected override void Init()
    {
        base.Init();
        BuildCardOptions();
    }

    private void BuildCardOptions()
    {
        Options = new List<string>();
        cardByOption.Clear();

        var actionDb = ActionDataBase.Instance?.ActionDictionary;
        var rougePlayer = RougeManager.Instance?.rougePlayer;
        if (actionDb == null || rougePlayer == null)
            return;

        var ownedActionIds = rougePlayer.GetAvailableBattleActionIdsOrDefault();
        var ownedSet = new HashSet<string>(ownedActionIds);
        var candidates = new List<ActionDefine>();

        foreach (var kv in actionDb)
        {
            ActionDefine action = kv.Value;
            if (action == null || string.IsNullOrEmpty(action.ID) || string.IsNullOrEmpty(action.Name))
                continue;
            if (ownedSet.Contains(action.ID))
                continue;

            candidates.Add(action);
        }

        int pickCount = Mathf.Min(3, candidates.Count);
        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, candidates.Count);
            ActionDefine picked = candidates[index];
            candidates.RemoveAt(index);

            string optionName = picked.Name;
            if (cardByOption.ContainsKey(optionName))
                optionName = $"{picked.Name} ({picked.ID})";

            cardByOption[optionName] = picked;
            Options.Add(optionName);
        }
    }

    public override void OnChoose(string option)
    {
        if (string.IsNullOrEmpty(option))
            return;
        if (!cardByOption.TryGetValue(option, out var action) || action == null)
            return;

        RougeManager.Instance?.rougePlayer?.AddAction(action.ID);
    }
}

public class TreasureEvent : EventDefine
{
    public TreasureEvent() : base("Treasure") { }

    public override void OnChoose(string option)
    {
        RougeManager.Instance?.rougePlayer?.gold?.GetGold(100);
    }
}

public class CurseFusion : EventDefine
{
    public Hero hero1;
    public Hero hero2;
    public CurseFusion() : base("CurseFusion") { }

    public override void OnChoose(string option)
    {
        RougePlayerUI.Instance.OpenAndCloseHeroPanel();
        RougePlayerUI.Instance.BuildRougeHeroDropDown();
    }
    public void MergeHero()
    {
        // Merge hero1 into hero2, then remove hero1 from the roster.
        if (hero1 == null || hero2 == null || hero1 == hero2)
            return;

        var rougePlayer = RougeManager.Instance?.rougePlayer;
        if (rougePlayer == null || rougePlayer.Heroes == null)
            return;

        if (hero1.skills != null)
        {
            if (hero2.skills == null)
                hero2.skills = new List<SkillDefine>();

            foreach (var skill in hero1.skills)
            {
                if (skill == null)
                    continue;

                // Avoid duplicate skill IDs on the merged hero.
                hero2.skills.Add(skill.Clone());
            }
        }

        int mergedMaxHp = Mathf.RoundToInt((hero1.MaxHP + hero2.MaxHP) * 0.5f);
        int hero1Current = hero1.CurrentHP != null ? hero1.CurrentHP.Value : hero1.MaxHP;
        int hero2Current = hero2.CurrentHP != null ? hero2.CurrentHP.Value : hero2.MaxHP;
        int mergedCurrentHp = Mathf.RoundToInt((hero1Current + hero2Current) * 0.5f);

        hero2.MaxHP = Mathf.Max(1, mergedMaxHp);
        if (hero2.CurrentHP == null)
            hero2.CurrentHP = new HPAttribute();
        hero2.CurrentHP.Set(Mathf.Clamp(mergedCurrentHp, 0, hero2.MaxHP));

        rougePlayer.Heroes.Remove(hero1, "CurseFusion.MergeHero");
    }
}
