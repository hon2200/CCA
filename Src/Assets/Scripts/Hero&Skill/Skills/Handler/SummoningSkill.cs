using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy skill that summons a unit by hero ID via PlayerManager.AddPlayer.
/// </summary>
public abstract class SummoningSkill : EnemySkill
{
    public SummoningSkill(string id, Player owner = null) : base(id, owner) { CDProgress = 1; }

    /// <summary>
    /// Summons a player by hero ID. Resolves the hero from HeroDataBase and adds it as an enemy AI via PlayerManager.AddPlayer.
    /// </summary>
    /// <param name="heroID">ID of the hero to summon.</param>
    /// <returns>The created player, or null if the hero ID is not found.</returns>
    public Player Summon(string heroID)
    {
        return Summon(heroID, GetInitialResourcesForSummon(heroID));
    }

    /// <summary>
    /// Summons a player by hero ID and immediately applies initial resources.
    /// initialResources format: [bullet, sword, reserved].
    /// </summary>
    public Player Summon(string heroID, List<int> initialResources)
    {
        if (string.IsNullOrEmpty(heroID))
            return null;

        if (!HeroDataBase.Instance.EnemyDictionary.TryGetValue(heroID, out var heroDefine))
        {
            Debug.LogWarning($"SummoningSkill: Enemy ID '{heroID}' not found in EnemyDataBase.");
            return null;
        }
        if (PlayerManager.Instance.ThereisAvailablePositions(true))
        {
            var spawned = PlayerManager.Instance.AddPlayer(isFriend: false, isHuman: false, heroDefine);
            ApplyInitialResources(spawned, initialResources);
            return spawned;
        }
        return null;
    }

    /// <summary>
    /// Override this in specific summoning skills if a summon needs custom spawn resources.
    /// </summary>
    protected virtual List<int> GetInitialResourcesForSummon(string heroID)
    {
        return new List<int> { 0, 0, 0 };
    }

    private static void ApplyInitialResources(Player player, List<int> initialResources)
    {
        if (player == null || player.status == null || player.status.resources == null)
            return;

        if (initialResources == null || initialResources.Count < 2)
            initialResources = new List<int> { 0, 0, 0 };

        int bullets = Mathf.Max(0, initialResources[0]);
        int swords = Mathf.Max(0, initialResources[1]);

        player.status.resources.Bullet.Set(bullets);
        player.status.resources.Sword.Set(swords);
    }
}
