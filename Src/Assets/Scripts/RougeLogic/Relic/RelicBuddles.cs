using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class DesertEagle : RelicDefine, IPhaseExitHandler
{
    public DesertEagle() : base("IMI Desert Eagle") { }
    public void ExitingPhase(Phase phase)
    {
        if(phase is ActionPhase)
        {
            foreach(var player in PlayerManager.Instance.FriendlyPlayers)
            {
                foreach(var action in player.action)
                {
                    if (action is AttackDefine attack)
                    {
                        if (action.Tags.Contains(ActionTag.Bullet))
                            attack.Damage += 1;
                    }
                }
            }
        }
    }
}

public class DragonSword : RelicDefine, IPhaseExitHandler
{
    public DragonSword() : base ("Longquan Sword") { }
    public void ExitingPhase(Phase phase)
    {
        if (phase is ActionPhase)
        {
            foreach (var player in PlayerManager.Instance.FriendlyPlayers)
            {
                foreach (var action in player.action)
                {
                    if (action is AttackDefine attack)
                    {
                        if (action.Tags.Contains(ActionTag.Sword))
                            attack.Damage += 1;
                    }
                }
            }
        }
    }
}

public class SummoningScripture : RelicDefine, IPhaseEnterHandler
{
    public SummoningScripture() : base("Summoning Scripture") { }
    public void OnPhase(Phase phase)
    {
        if (BattleManager.Instance.Turn.Value == 30)
            foreach(var player in PlayerManager.Instance.HostilePlayers)
            {
                player.status.HP.Damage(30, null, player, null);
            }
    }
}

public class NamelessBrokenSword : RelicDefine, IPhaseEnterHandler
{
    public NamelessBrokenSword() : base("Nameless Broken Sword") { }
    public void OnPhase(Phase phase)
    {
        if(phase is StartPhase)
        {
            foreach (var player in PlayerManager.Instance.FriendlyPlayers)
            {
                bool hasSkill = false;
                foreach(var skill in player.hero.skills)
                {
                    if (skill is NamelessBrokenSwordSkill)
                        hasSkill = true;
                }
                if (!hasSkill)
                    player.hero.skills.Add(new NamelessBrokenSwordSkill());
            }
        }
    }
}

public class HolyLightProtection : RelicDefine, IPhaseEnterHandler
{
    public HolyLightProtection() : base("Holy Light Protection") { }
    public bool isUsed = false;
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
        {
            foreach (var player in PlayerManager.Instance.FriendlyPlayers)
            {
                player.status.buffs.Apply(new Invincible(3, player));
            }
        }
    }
}

public class IronHat : RelicDefine, IDamagedHandler
{
    public IronHat() : base("Iron Hat") { }
    public void OnDamaged(Player attacker, Player victim, int amount, out int block)
    {
        if (PlayerManager.Instance.FriendlyPlayers.Contains(victim))
            block = 1;
        else
            block = 0;
    }
}

public class ChaosHeart : RelicDefine, IDamagingHandler
{
    public ChaosHeart() : base("Chaos Heart") { }
    public void OnDamaging(Player attacker, Player victim, int amount, out int finalDamage)
    {
        if (attacker == null && PlayerManager.Instance.HostilePlayers.Contains(victim))
            finalDamage = amount;
        else
            finalDamage = 0;
    }
}

public class AmmoProductionLine : RelicDefine, IActionModifier
{
    public AmmoProductionLine() : base("Ammo Production Line"){}
    public void ModifyAction(Player player_ = null)
    {
        foreach(var player in PlayerManager.Instance.FriendlyPlayers)
        {
            foreach(var action in player.action)
            {
                if(action is SupplyDefine supply && supply.Tags.Contains(ActionTag.Bullet))
                {
                    supply.SupplyNumber[1] += 1;
                }
            }
        }
    }
}

public class AutoPress : RelicDefine, IActionModifier
{
    public AutoPress() : base("Auto Press") { }
    public void ModifyAction(Player player_ = null)
    {
        foreach (var player in PlayerManager.Instance.FriendlyPlayers)
        {
            foreach (var action in player.action)
            {
                if (action is SupplyDefine supply && supply.Tags.Contains(ActionTag.Sword))
                {
                    supply.SupplyNumber[2] += 1;
                }
            }
        }
    }
}

public class ShockGlove : RelicDefine, IStunningHandler
{
    public ShockGlove(): base("Shock Glove") { }
    public void OnStunning(Player attacker, Player victim)
    {
        victim.status.HP.Damage(1, attacker, victim, null);
    }
}

public class EarthbreakerHammer : RelicDefine, IActionModifier
{
    public EarthbreakerHammer() : base("Earthbreaker Hammer") { }
    public void ModifyAction(Player player_ = null)
    {
        foreach(var player in PlayerManager.Instance.FriendlyPlayers)
        {
            foreach(var action in player.action)
            {
                if(action is AttackDefine attack)
                {
                    int n = player.status.HP.Value / 10;
                    attack.Level += (n - 1) / 2f;
                }
            }
        }
    }
}

public class DepletedUraniumRose : RelicDefine, IDeathHandler
{
    public DepletedUraniumRose() : base("Depleted Uranium Rose") { }
    public bool OnDeath(Player player)
    {
        if(PlayerManager.Instance.FriendlyPlayers.Contains(player))
        {
            foreach (var enemy in PlayerManager.Instance.HostilePlayers)
            {
                enemy.status.HP.Damage(3, player, enemy, null);
            }
        }
        return false;
    }
}

public class UndyingHeart : RelicDefine, IBattleEndHandler
{
    public UndyingHeart() : base("Undying Heart") { }
    public void OnBattleEnd(Player player)
    {
        foreach(var friend in PlayerManager.Instance.FriendlyPlayers)
        {
            friend.status.HP.Heal(2);
        }
    }
}

public class MorticianLicense : RelicDefine, IBattleEndHandler, IDeathHandler
{
    public MorticianLicense() : base("Morticien License") { counts.Add(0); }
    public bool OnDeath(Player player)
    {
        counts[0] += 2;
        return false; 
    }
    public void OnBattleEnd(Player player = null)
    {
        RougeManager.Instance?.rougePlayer?.gold?.GetGold(counts[0]);
        counts[0] = 0;
    }
}

public class TaieAlchemyFurnace : RelicDefine
{
    public TaieAlchemyFurnace() : base("Taie's Alchemy Furnace") { }
    public override void OnPickup()
    {
        var rougePlayer = RougeManager.Instance?.rougePlayer;
        if (rougePlayer != null)
            rougePlayer.PotionMax += 2;
    }
}



//???
public class NamelessBrokenSwordSkill : SkillDefine
{
    public NamelessBrokenSwordSkill() : base("Nameless Broken Sword") { }
}

