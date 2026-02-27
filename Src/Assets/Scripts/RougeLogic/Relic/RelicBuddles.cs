using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class DesertEagle : Relic, IPhaseExitHandler
{
    public DesertEagle() : base("IMI Desert Eagle") { }
    public void ExitingPhase(Phase phase, Player player_ = null)
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

public class DragonSword : Relic, IPhaseExitHandler
{
    public DragonSword() : base ("Longquan Sword") { }
    public void ExitingPhase(Phase phase, Player player_ = null)
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

public class SummoningScripture : Relic, IPhaseEnterHandler
{
    public SummoningScripture() : base("Summoning Scripture") { }
    public void OnPhase(Phase phase, Player player_ = null)
    {
        if (BattleManager.Instance.Turn.Value == 30)
            foreach(var player in PlayerManager.Instance.HostilePlayers)
            {
                player.status.HP.Damage(30, null, player, null);
            }
    }
}

public class NamelessBrokenSword : Relic, IPhaseEnterHandler
{
    public NamelessBrokenSword() : base("Nameless Broken Sword") { }
    public void OnPhase(Phase phase, Player player_)
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

public class HolyLightProtection : Relic, IPhaseEnterHandler
{
    public HolyLightProtection() : base("Holy Light Protection") { }
    public bool isUsed = false;
    public void OnPhase(Phase phase, Player player_)
    {
        if (phase is StartPhase)
        {
            foreach (var player in PlayerManager.Instance.FriendlyPlayers)
            {
                player.status.buffs.Add(new Invincible(3));
            }
        }
    }
}

public class IronHat : Relic, IDamagedHandler
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

public class ChaosHeart : Relic, IDamagingHandler
{
    public ChaosHeart() : base("Chaos Heart") { }
    public void OnDamaging(Player attacker, Player victim, int amount, out int increasedDamage)
    {
        if (attacker == null && PlayerManager.Instance.HostilePlayers.Contains(victim))
            increasedDamage = amount;
        else
            increasedDamage = 0;
    }
}












//ดýะด
public class NamelessBrokenSwordSkill : Skill
{
    public NamelessBrokenSwordSkill() : base("Nameless Broken Sword") { }
}

public class Invincible : Buff
{
    public Invincible(int value) : base("Invincible", value, false) { }
}