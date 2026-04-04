using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Relic-offer UI: pool/pick and prefab spawning. Uses <see cref="RougeManager.Instance"/>.<c>rougePlayer</c> when no player is passed.
/// </summary>
public class RelicOfferManager : MonoBehaviour
{
    [Header("Relics Offer UI")]
    [SerializeField] private Transform relicContainer;
    [SerializeField] private float relicSpacing = 2f;
    [SerializeField] private GameObject relicPrefab;

    private static RougePlayer ResolvePlayer(RougePlayer player)
    {
        if (player != null)
            return player;
        return RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null;
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
    /// Uses serialized container / prefab / spacing and <see cref="RougeManager"/> run state.
    /// </summary>
    public List<GameObject> DisplayThreeRandomRelicsNotOwned(RougePlayer player = null, bool clearContainer = true)
    {
        return DisplayThreeRandomRelicsNotOwned(relicContainer, relicSpacing, relicPrefab, ResolvePlayer(player), clearContainer);
    }

    /// <summary>
    /// Instantiates up to three random relics the player does not have, laid out on <paramref name="container"/> with <paramref name="spacing"/> on X (same pattern as <see cref="RelicDisplay"/>).
    /// Uses <see cref="RuntimeRelic"/> + <see cref="RelicUI"/> on <paramref name="relicPrefab"/>.
    /// </summary>
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
            Debug.LogWarning("[RelicOfferManager] DisplayThreeRandomRelicsNotOwned: container or relicPrefab is null.");
            return spawned;
        }

        var targetPlayer = player ?? ResolvePlayer(null);
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
}
