using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Card-offer UI: pool/pick logic and spawning under a configured parent. Uses <see cref="RougeManager.Instance"/>.<c>rougePlayer</c> when no player is passed.
/// </summary>
public class CardOfferManager : MonoBehaviour
{
    [Header("Cards Offer UI")]
    [SerializeField] private Transform cardParent;
    [SerializeField] private Vector3 lineCenter = new Vector3(0f, -6f, 0f);
    [SerializeField] private float lineSpacing = 1.5f;
    [SerializeField] private CardArranger cardArranger;

    private static RougePlayer ResolvePlayer(RougePlayer player)
    {
        if (player != null)
            return player;
        return RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null;
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
    /// Uses serialized parent / layout / arranger and <see cref="RougeManager"/> run state.
    /// </summary>
    public List<GameObject> DisplayThreeRandomCardsNotInAvailableActions(RougePlayer player = null, bool clearParent = true)
    {
        var ca = cardArranger != null ? cardArranger : CardPresentSystem.Instance != null ? CardPresentSystem.Instance.CardArranger : null;
        return DisplayThreeRandomCardsNotInAvailableActions(cardParent, lineCenter, lineSpacing, ResolvePlayer(player), ca, clearParent);
    }

    /// <summary>
    /// Creates up to three cards (via <see cref="CardPresentSystem.CreateCard"/>) under <paramref name="parent"/>, then lays them out with <see cref="CardArranger.ArrangeLine"/>
    /// using <paramref name="lineCenter"/> and <paramref name="lineSpacing"/>.
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
            Debug.LogWarning("[CardOfferManager] DisplayThreeRandomCardsNotInAvailableActions: parent is null.");
            return spawned;
        }

        if (CardPresentSystem.Instance == null)
        {
            Debug.LogWarning("[CardOfferManager] DisplayThreeRandomCardsNotInAvailableActions: CardPresentSystem.Instance is null.");
            return spawned;
        }

        var ca = arranger ?? CardPresentSystem.Instance.CardArranger;
        if (ca == null)
        {
            Debug.LogWarning("[CardOfferManager] DisplayThreeRandomCardsNotInAvailableActions: CardArranger is null.");
            return spawned;
        }

        var targetPlayer = player ?? ResolvePlayer(null);
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
}
