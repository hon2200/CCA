using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy skill that summons a unit by hero ID via PlayerManager.AddPlayer.
/// </summary>
public abstract class TransformingSkill : EnemySkill
{
    public TransformingSkill(string id, Player owner = null) : base(id, owner) { CDProgress = 1; }

    /// <summary>
    /// Summons a player by hero ID. Resolves the hero from HeroDataBase and adds it as an enemy AI via PlayerManager.AddPlayer.
    /// </summary>
    /// <param name="heroID">ID of the hero to summon.</param>
    /// <returns>The created player, or null if the hero ID is not found.</returns>
    public void Transform(string heroID)
    {
        if (Owner == null || HeroDataBase.Instance?.EnemyDictionary == null)
            return;

        HeroDataBase.Instance.EnemyDictionary.TryGetValue(heroID, out var enemy);
        if (enemy == null)
            return;

        Hero hero = new(Owner, enemy);
        Owner.SetHero(hero);

        // Owner.playerUIText.spriteRenderer : Reset the sprite after transform.
        Owner.playerUIText?.InitializeImagesFromLibrary();
        if (Owner.playerUIText?.spriteRenderer != null)
            Owner.playerUIText.spriteRenderer.color = Color.white;
    }

    /// <summary>
    /// Summons a player by hero ID and immediately applies initial resources.
    /// initialResources format: [bullet, sword, reserved].
    /// </summary>
   
}
