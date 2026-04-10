using System;
using System.Collections.Generic;
using UnityEngine;

public class RougePlayer
{
    public GoldAttribute gold;
    public ObservableList<RelicDefine> Relics;
    public ObservableList<Hero> Heroes;
    public List<Potion> Potions;
    public int PotionMax;

    /// <summary>
    /// Custom battle action IDs for this run. When null or empty, <see cref="GetAvailableBattleActionIdsOrDefault"/> uses <see cref="GetDefaultBattleAvailableActionIds"/>.
    /// </summary>
    public List<string> availableAction;

    /// <summary>
    /// Same meaning as <see cref="Player.AvailableActions"/> / <see cref="Player.Initialize"/> when no explicit list is passed:
    /// all basic action IDs from <see cref="ActionDataBase"/>. Used for non-tutorial runs (e.g. roguelike) so defaults live here.
    /// </summary>
    public static List<string> GetDefaultBattleAvailableActionIds()
    {
        var list = new List<string>();
        if (ActionDataBase.Instance == null || ActionDataBase.Instance.ActionDictionary == null)
            return list;

        foreach (var action in ActionDataBase.Instance.ActionDictionary.Values)
        {
            if (action != null && action.isBasic)
                list.Add(action.ID);
        }
        return list;
    }

    /// <summary>
    /// Resolved battle action IDs: <see cref="availableAction"/> when non-null and non-empty; otherwise a fresh copy of <see cref="GetDefaultBattleAvailableActionIds"/>.
    /// </summary>
    public List<string> GetAvailableBattleActionIdsOrDefault()
    {
        if (availableAction != null && availableAction.Count > 0)
            return new List<string>(availableAction);
        return GetDefaultBattleAvailableActionIds();
    }

    /// <summary>
    /// Adds an action ID. If <see cref="availableAction"/> is null or empty, it is first filled from <see cref="GetDefaultBattleAvailableActionIds"/> (copy), then the id is added if missing.
    /// </summary>
    public void AddAction(string actionId)
    {
        if (string.IsNullOrEmpty(actionId))
            return;

        if (availableAction == null || availableAction.Count == 0)
            availableAction = new List<string>(GetDefaultBattleAvailableActionIds());

        if (!availableAction.Contains(actionId))
            availableAction.Add(actionId);
    }

    /// <summary>
    /// Removes an action ID. If <see cref="availableAction"/> is null or empty, a copy of the default list is created first, then the id is removed.
    /// </summary>
    /// <returns>True if an element was removed.</returns>
    public bool DeleteAction(string actionId)
    {
        if (string.IsNullOrEmpty(actionId))
            return false;

        if (availableAction == null || availableAction.Count == 0)
            availableAction = new List<string>(GetDefaultBattleAvailableActionIds());

        return availableAction.Remove(actionId);
    }

    public RougePlayer()
    {
        Init();
    }
    public void Init()
    {
        if (Relics == null) Relics = new ObservableList<RelicDefine>();
        if (Heroes == null) Heroes = new ();
        if (Potions == null) Potions = new List<Potion>();
        PotionMax = 3;
        gold = new();
        gold.SetGold(100);
        availableAction = null;
    }

    /// <summary>
    /// Hero ID for Wukong in <see cref="HeroDataBase.HeroDictionary"/> / Hero.json.
    /// </summary>
    public const string WukongHeroId = "Wukong";

    /// <summary>
    /// Resets this run with <see cref="Init"/> then adds two Wukong heroes (duplicates allowed).
    /// Call after <see cref="HeroDataBase"/> is ready (e.g. from scene load or a bootstrap).
    /// </summary>
    /// <returns>True if both heroes were added.</returns>
    public bool InitializeWithTwoWukongHeroes()
    {
        Init();

        if (HeroDataBase.Instance == null || HeroDataBase.Instance.HeroDictionary == null)
        {
            Debug.LogWarning("InitializeWithTwoWukongHeroes: HeroDataBase is not ready.");
            return false;
        }

        if (!HeroDataBase.Instance.HeroDictionary.TryGetValue(WukongHeroId, out var wukongDefine) || wukongDefine == null)
        {
            Debug.LogWarning($"InitializeWithTwoWukongHeroes: hero '{WukongHeroId}' not found in HeroDictionary.");
            return false;
        }

        Hero first = RecruitHero(wukongDefine, cost: 0, allowDuplicate: true);
        Hero second = RecruitHero(wukongDefine, cost: 0, allowDuplicate: true);
        return first != null && second != null;
    }

    #region Hero Management

    public bool HasHero(string heroID)
    {
        if (Heroes == null || string.IsNullOrEmpty(heroID))
            return false;

        foreach (var hero in Heroes)
        {
            if (hero != null && hero.ID == heroID)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Recruit a hero and optionally pay a cost.
    /// Returns the recruited hero instance, or null on failure.
    /// </summary>
    public Hero RecruitHero(HeroDefine heroDefine, int cost = 0, bool allowDuplicate = false)
    {
        if (heroDefine == null)
        {
            Debug.LogWarning("RecruitHero failed: heroDefine is null.");
            return null;
        }

        if (!allowDuplicate && HasHero(heroDefine.ID))
        {
            Debug.LogWarning($"RecruitHero skipped: already has hero {heroDefine.ID}.");
            return null;
        }

        if (Heroes == null)
            Heroes = new ();

        // Hero constructor currently does not require a valid Player reference for meta usage.
        Hero newHero = new Hero(null, heroDefine);
        Heroes.Add(newHero, "RecruitHero");
        return newHero;
    }

    /// <summary>
    /// Dismiss a hero by reference, optionally granting a refund.
    /// </summary>
    public bool DismissHero(Hero hero)
    {
        if (Heroes == null || hero == null)
            return false;

        bool removed = Heroes.Remove(hero, "DismissHero");
        if (!removed)
            return false;

        return true;
    }

    #endregion

    #region Potion Management

    public bool HasPotion(string potionID)
    {
        if (Potions == null || string.IsNullOrEmpty(potionID))
            return false;

        foreach (var potion in Potions)
        {
            if (potion != null && potion.ID == potionID)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Add a potion to the inventory.
    /// Caller is responsible for constructing the potion (e.g. via a library).
    /// </summary>
    public bool GetPotion(Potion potion)
    {
        if (potion == null)
            return false;

        if (Potions == null)
            Potions = new List<Potion>();

        Potions.Add(potion);
        return true;
    }

    /// <summary>
    /// Discard a potion instance from the inventory without using it.
    /// </summary>
    public bool DiscardPotion(Potion potion)
    {
        if (Potions == null || potion == null)
            return false;

        return Potions.Remove(potion);
    }

    /// <summary>
    /// Discard a potion by ID from the inventory without using it.
    /// </summary>
    public bool DiscardPotion(string potionID)
    {
        if (Potions == null || string.IsNullOrEmpty(potionID))
            return false;

        Potion target = null;
        foreach (var potion in Potions)
        {
            if (potion != null && potion.ID == potionID)
            {
                target = potion;
                break;
            }
        }

        if (target == null)
            return false;

        return DiscardPotion(target);
    }

    /// <summary>
    /// Use (consume) a potion instance.
    /// This ONLY removes it from the inventory and returns it for external effect handling.
    /// </summary>
    public Potion UsePotion(Potion potion)
    {
        if (Potions == null || potion == null)
            return null;

        if (!Potions.Remove(potion))
            return null;

        return potion;
    }

    /// <summary>
    /// Use (consume) a potion by ID.
    /// Returns the consumed potion, or null if not found.
    /// </summary>
    public Potion UsePotion(string potionID)
    {
        if (Potions == null || string.IsNullOrEmpty(potionID))
            return null;

        Potion target = null;
        foreach (var potion in Potions)
        {
            if (potion != null && potion.ID == potionID)
            {
                target = potion;
                break;
            }
        }

        if (target == null)
            return null;

        return UsePotion(target);
    }

    #endregion

    #region Relic Management

    public bool HasRelic(string relicID)
    {
        if (Relics == null || string.IsNullOrEmpty(relicID))
            return false;

        foreach (var relic in Relics)
        {
            if (relic != null && relic.ID == relicID)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Obtain a relic and optionally pay a cost.
    /// </summary>
    public bool GetRelic(RelicDefine relic, int cost = 0, bool allowDuplicate = true)
    {
        if (relic == null)
            return false;

        if (Relics == null)
            Relics = new ObservableList<RelicDefine>();

        if (!allowDuplicate && HasRelic(relic.ID))
            return false;

        Relics.Add(relic, "GetRelic");
        return true;
    }

    /// <summary>
    /// Lose a relic instance.
    /// </summary>
    public bool LoseRelic(RelicDefine relic)
    {
        if (Relics == null || relic == null)
            return false;

        return Relics.Remove(relic, "LoseRelic");
    }

    /// <summary>
    /// Lose a relic by ID.
    /// </summary>
    public bool LoseRelic(string relicID)
    {
        if (Relics == null || string.IsNullOrEmpty(relicID))
            return false;

        RelicDefine target = null;
        foreach (var relic in Relics)
        {
            if (relic != null && relic.ID == relicID)
            {
                target = relic;
                break;
            }
        }

        if (target == null)
            return false;

        return LoseRelic(target);
    }

    #endregion

}
