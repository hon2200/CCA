using UnityEngine;

/// <summary>
/// Enemy skill that summons a unit by hero ID via PlayerManager.AddPlayer.
/// </summary>
public abstract class SummoningSkill : EnemySkill
{
    public SummoningSkill(string id, Player owner = null) : base(id, owner) { }

    /// <summary>
    /// Summons a player by hero ID. Resolves the hero from HeroDataBase and adds it as an enemy AI via PlayerManager.AddPlayer.
    /// </summary>
    /// <param name="heroID">ID of the hero to summon.</param>
    /// <returns>The created player, or null if the hero ID is not found.</returns>
    public Player Summon(string heroID)
    {
        if (string.IsNullOrEmpty(heroID))
            return null;

        if (!HeroDataBase.Instance.HeroDictionary.TryGetValue(heroID, out var heroDefine))
        {
            Debug.LogWarning($"SummoningSkill: Hero ID '{heroID}' not found in HeroDataBase.");
            return null;
        }

        return PlayerManager.Instance.AddPlayer(isFriend: false, isHuman: false, heroDefine);
    }
}
