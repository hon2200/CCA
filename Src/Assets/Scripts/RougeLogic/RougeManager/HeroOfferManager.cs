using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hero-offer UI: pool/pick and prefab spawning. Uses <see cref="RougeManager.Instance"/>.<c>rougePlayer</c> when no player is passed.
/// </summary>
public class HeroOfferManager : MonoBehaviour
{
    [Header("Heroes Offer UI")]
    [SerializeField] private Transform heroContainer;
    [SerializeField] private float heroSpacing = 2.5f;
    [SerializeField] private GameObject heroPrefab;

    private static RougePlayer ResolvePlayer(RougePlayer player)
    {
        if (player != null)
            return player;
        return RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null;
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
    /// Uses serialized container / prefab / spacing and <see cref="RougeManager"/> run state.
    /// </summary>
    public List<GameObject> DisplayThreeRandomHeroesNotOwned(RougePlayer player = null, bool clearContainer = true)
    {
        return DisplayThreeRandomHeroesNotOwned(heroContainer, heroSpacing, heroPrefab, ResolvePlayer(player), clearContainer);
    }

    /// <summary>
    /// Instantiates up to three random heroes the player does not have, laid out on <paramref name="container"/> with <paramref name="spacing"/> on local X (same pattern as <see cref="RelicOfferManager.DisplayThreeRandomRelicsNotOwned(Transform, float, GameObject, RougePlayer, bool)"/>).
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
            Debug.LogWarning("[HeroOfferManager] DisplayThreeRandomHeroesNotOwned: container or heroPrefab is null.");
            return spawned;
        }

        var targetPlayer = player ?? ResolvePlayer(null);
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
