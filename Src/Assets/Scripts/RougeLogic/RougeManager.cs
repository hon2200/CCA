using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Roguelike meta + offer UI helpers (relics, cards, etc.).
/// </summary>
public class RougeManager : MonoSingleton<RougeManager>
{
    public RougePlayer rougePlayer;

    private void Awake()
    {
        rougePlayer = new RougePlayer();
        rougePlayer.Relics.OnListChanged = (list, message) =>
        {
            if (RelicDisplay.Instance != null)
                RelicDisplay.Instance.RefreshDisplay();
        };
    }

    /// <summary>
    /// Same relic pool as <see cref="RelicDropDownController"/>: registered in <see cref="RelicDatabase"/> and has a template in <see cref="RelicLiberary"/>.
    /// </summary>
    public static List<RelicDefine> BuildRelicOfferPool(RougePlayer player)
    {
        var result = new List<RelicDefine>();
        if (RelicDatabase.Instance == null || RelicDatabase.Instance.RelicDictionary == null)
            return result;

        var library = RelicLiberary.Instance?.RelicDictionary;
        foreach (var relic in RelicDatabase.Instance.RelicDictionary.Values)
        {
            if (relic == null)
                continue;
            if (library != null && !library.ContainsKey(relic.ID))
                continue;
            if (player != null && player.HasRelic(relic.ID))
                continue;
            result.Add(relic);
        }
        return result;
    }

    /// <summary>
    /// Picks up to three random relics from the offer pool that the player does not own.
    /// </summary>
    public static List<RelicDefine> PickThreeRandomRelicsNotOwned(RougePlayer player)
    {
        var pool = BuildRelicOfferPool(player);
        var picked = new List<RelicDefine>();
        int n = Mathf.Min(3, pool.Count);
        for (int k = 0; k < n && pool.Count > 0; k++)
        {
            int i = Random.Range(0, pool.Count);
            picked.Add(pool[i]);
            pool.RemoveAt(i);
        }
        return picked;
    }

    /// <summary>
    /// Instantiates up to three random relics the player does not have, laid out on <paramref name="container"/> with <paramref name="spacing"/> on X (same pattern as <see cref="RelicDisplay"/>).
    /// Uses <see cref="RuntimeRelic"/> + <see cref="RelicUI"/> on <paramref name="relicPrefab"/>.
    /// </summary>
    /// <param name="player">Defaults to <see cref="rougePlayer"/> when null.</param>
    /// <param name="clearContainer">When true, destroys existing children of <paramref name="container"/> first.</param>
    public List<GameObject> DisplayThreeRandomRelicsNotOwned(
        Transform container,
        float spacing,
        GameObject relicPrefab,
        RougePlayer player = null,
        bool clearContainer = true)
    {
        var spawned = new List<GameObject>();
        if (container == null || relicPrefab == null)
        {
            Debug.LogWarning("[RougeManager] DisplayThreeRandomRelicsNotOwned: container or relicPrefab is null.");
            return spawned;
        }

        var targetPlayer = player ?? rougePlayer;
        var relics = PickThreeRandomRelicsNotOwned(targetPlayer);

        if (clearContainer)
        {
            for (int c = container.childCount - 1; c >= 0; c--)
                Destroy(container.GetChild(c).gameObject);
        }

        for (int i = 0; i < relics.Count; i++)
        {
            var relic = relics[i];
            if (relic == null)
                continue;

            var instance = Instantiate(relicPrefab, container);
            instance.transform.localPosition = new Vector3(i * spacing, 0f, 0f);

            var runtimeRelic = instance.GetComponent<RuntimeRelic>();
            if (runtimeRelic != null)
            {
                runtimeRelic.relicDefine = relic;
                if (RelicLiberary.Instance != null && RelicLiberary.Instance.RelicDictionary != null)
                {
                    RelicLiberary.Instance.RelicDictionary.TryGetValue(relic.ID, out var relicTemplete);
                    if (relicTemplete == null)
                        RelicLiberary.Instance.RelicDictionary.TryGetValue("Default", out relicTemplete);
                    runtimeRelic.relicTemplete = relicTemplete;
                }
            }

            var relicUI = instance.GetComponent<RelicUI>();
            if (relicUI != null)
                relicUI.InitializeFromLibrary();

            spawned.Add(instance);
        }

        return spawned;
    }

    /// <summary>
    /// Card templates in <see cref="CardLiberary"/> whose action ID is not in the player's effective available actions
    /// (<see cref="RougePlayer.GetAvailableBattleActionIdsOrDefault"/>), and exist in <see cref="ActionDataBase"/> — same validity as <see cref="CardPresentSystem.CreateCards"/>.
    /// </summary>
    public static List<CardTemplete> BuildCardOfferPoolNotInAvailableActions(RougePlayer player)
    {
        var result = new List<CardTemplete>();
        if (player == null || CardLiberary.Instance?.CardDictionary == null || ActionDataBase.Instance?.ActionDictionary == null)
            return result;

        var unlocked = new HashSet<string>(player.GetAvailableBattleActionIdsOrDefault());

        foreach (var kv in CardLiberary.Instance.CardDictionary)
        {
            string actionId = kv.Key;
            var template = kv.Value;
            if (template == null)
                continue;
            if (unlocked.Contains(actionId))
                continue;
            if (!ActionDataBase.Instance.ActionDictionary.ContainsKey(actionId))
                continue;
            result.Add(template);
        }

        return result;
    }

    /// <summary>
    /// Picks up to three random card templates from <see cref="BuildCardOfferPoolNotInAvailableActions"/>.
    /// </summary>
    public static List<CardTemplete> PickThreeRandomCardsNotInAvailableActions(RougePlayer player)
    {
        var pool = BuildCardOfferPoolNotInAvailableActions(player);
        var picked = new List<CardTemplete>();
        int n = Mathf.Min(3, pool.Count);
        for (int k = 0; k < n && pool.Count > 0; k++)
        {
            int i = Random.Range(0, pool.Count);
            picked.Add(pool[i]);
            pool.RemoveAt(i);
        }
        return picked;
    }

    /// <summary>
    /// Creates up to three cards (via <see cref="CardPresentSystem.CreateCard"/>) under <paramref name="parent"/>, then lays them out with <see cref="CardArranger.ArrangeLine"/>
    /// using <paramref name="lineCenter"/> and <paramref name="lineSpacing"/> (same math as in <see cref="CardPresentSystem"/> / <see cref="CardViewSystem"/>).
    /// Restores the shared <see cref="CardArranger"/> hand list and center/spacing after arranging so the combat hand is unaffected.
    /// </summary>
    public List<GameObject> DisplayThreeRandomCardsNotInAvailableActions(
        Transform parent,
        Vector3 lineCenter,
        float lineSpacing,
        RougePlayer player = null,
        CardArranger arranger = null,
        bool clearParent = true)
    {
        var spawned = new List<GameObject>();
        if (parent == null)
        {
            Debug.LogWarning("[RougeManager] DisplayThreeRandomCardsNotInAvailableActions: parent is null.");
            return spawned;
        }

        if (CardPresentSystem.Instance == null)
        {
            Debug.LogWarning("[RougeManager] DisplayThreeRandomCardsNotInAvailableActions: CardPresentSystem.Instance is null.");
            return spawned;
        }

        var ca = arranger ?? CardPresentSystem.Instance.CardArranger;
        if (ca == null)
        {
            Debug.LogWarning("[RougeManager] DisplayThreeRandomCardsNotInAvailableActions: CardArranger is null.");
            return spawned;
        }

        var targetPlayer = player ?? rougePlayer;
        var templates = PickThreeRandomCardsNotInAvailableActions(targetPlayer);

        if (clearParent)
        {
            for (int c = parent.childCount - 1; c >= 0; c--)
                Destroy(parent.GetChild(c).gameObject);
        }

        foreach (var template in templates)
        {
            if (template == null)
                continue;
            var go = CardPresentSystem.Instance.CreateCard(template, parent);
            if (go != null)
                spawned.Add(go);
        }

        if (spawned.Count == 0)
            return spawned;

        var prevHand = ca.handCards;
        var prevCenter = ca.CenterPoint;
        float prevSpacing = ca.spacing;

        ca.handCards = spawned;
        ca.CenterPoint = lineCenter;
        ca.spacing = lineSpacing;
        ca.ArrangeLine();

        ca.handCards = prevHand;
        ca.CenterPoint = prevCenter;
        ca.spacing = prevSpacing;

        return spawned;
    }

    /// <summary>
    /// Same hero pool as <see cref="DropDownController"/>: <see cref="HeroDataBase"/> heroes that have a template in <see cref="HeroLiberary"/>, excluding ones <paramref name="player"/> already has.
    /// </summary>
    public static List<HeroDefine> BuildHeroOfferPool(RougePlayer player)
    {
        var result = new List<HeroDefine>();
        if (HeroDataBase.Instance == null || HeroDataBase.Instance.HeroDictionary == null)
            return result;

        var library = HeroLiberary.Instance?.HeroDictionary;
        foreach (var heroDefine in HeroDataBase.Instance.HeroDictionary.Values)
        {
            if (heroDefine == null)
                continue;
            if (library != null && !library.ContainsKey(heroDefine.ID))
                continue;
            if (player != null && player.HasHero(heroDefine.ID))
                continue;
            result.Add(heroDefine);
        }

        return result;
    }

    /// <summary>
    /// Picks up to three random heroes from the offer pool that the player does not have.
    /// </summary>
    public static List<HeroDefine> PickThreeRandomHeroesNotOwned(RougePlayer player)
    {
        var pool = BuildHeroOfferPool(player);
        var picked = new List<HeroDefine>();
        int n = Mathf.Min(3, pool.Count);
        for (int k = 0; k < n && pool.Count > 0; k++)
        {
            int i = Random.Range(0, pool.Count);
            picked.Add(pool[i]);
            pool.RemoveAt(i);
        }
        return picked;
    }

    /// <summary>
    /// Instantiates up to three random heroes the player does not have, laid out on <paramref name="container"/> with <paramref name="spacing"/> on local X (same pattern as <see cref="DisplayThreeRandomRelicsNotOwned"/>).
    /// Expects <paramref name="heroPrefab"/> with <see cref="RuntimeHero"/>; sets <see cref="HeroDefine"/> and <see cref="HeroTemplete"/> from libraries.
    /// </summary>
    public List<GameObject> DisplayThreeRandomHeroesNotOwned(
        Transform container,
        float spacing,
        GameObject heroPrefab,
        RougePlayer player = null,
        bool clearContainer = true)
    {
        var spawned = new List<GameObject>();
        if (container == null || heroPrefab == null)
        {
            Debug.LogWarning("[RougeManager] DisplayThreeRandomHeroesNotOwned: container or heroPrefab is null.");
            return spawned;
        }

        var targetPlayer = player ?? rougePlayer;
        var heroes = PickThreeRandomHeroesNotOwned(targetPlayer);

        if (clearContainer)
        {
            for (int c = container.childCount - 1; c >= 0; c--)
                Destroy(container.GetChild(c).gameObject);
        }

        for (int i = 0; i < heroes.Count; i++)
        {
            var define = heroes[i];
            if (define == null)
                continue;

            var instance = Instantiate(heroPrefab, container);
            instance.transform.localPosition = new Vector3(i * spacing, 0f, 0f);

            var runtimeHero = instance.GetComponent<RuntimeHero>();
            if (runtimeHero != null)
            {
                runtimeHero.heroDefine = define;
                if (HeroLiberary.Instance != null && HeroLiberary.Instance.HeroDictionary != null)
                    HeroLiberary.Instance.HeroDictionary.TryGetValue(define.ID, out runtimeHero.heroTemplete);
            }

            spawned.Add(instance);
        }

        return spawned;
    }
}