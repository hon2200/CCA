using System;
using System.Collections.Generic;
using UnityEngine;
using static BuffOperator;

//All The Buffs
public class Stunned : Buff, IPhaseEnterHandler
{
    public Stunned(int duration, Player attacker, Player victim) : base("Stunned", duration, true, victim)
    {
        foreach(var relic in RougeManager.Instance.rougePlayer.Relics)
        {
            if(relic is IStunningHandler stunRelic)
            {
                stunRelic.OnStunning(attacker, victim);
            }
        }
        if (attacker != null)
            foreach(var skill in attacker.hero.skills)
            {
                if(skill is IStunningHandler stunSkill)
                {
                    stunSkill.OnStunning(attacker,victim);
                }
            }
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase && Owner != null)
        {
            foreach (var action in Owner.AvailableActions)
            {
                if (ActionUtil.IsAction<DefendDefine>(action) || action == "provoke") ;
                else
                {
                    Owner.ForbiddenActions.Add(action);
                }
            }
        }
    }
    public override void Fade()
    {
        Value -= 1;
        if (Value <= 0)
            Owner.status.buffs.Remove(this);
    }
}

public class Invincible : Buff
{
    public Invincible(int value, Player thisPlayer) : base("Invincible", value, false, thisPlayer) { }
}

public class DamagingOperator : BuffOperator, IDamagingHandler
{
    public DamagingOperator(List<Step> steps, Player player) : base("Damaging Operator", steps, player) { }
    public DamagingOperator(Step step, Player player) : base("Damaging Operator", step, player) { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int outcome)
    {
        outcome = ApplyOperatorInt(damage);
    }
}

public class DamagedOperator : BuffOperator, IDamagedHandler
{
    public DamagedOperator(List<Step> steps, Player player) : base("Damaged Operator", steps, player) { }
    public DamagedOperator(Step step, Player player) : base("Damaged Operator", step, player) { }
    public void OnDamaged(Player attacker, Player victim, int damage, out int outcome)
    {
        outcome = ApplyOperatorInt(damage);
    }
}

public class AttackingLevelOperator : BuffOperator, ICombatHandler
{
    public AttackingLevelOperator(List<Step> steps, Player player) : base("Attacking Level Operator", steps, player) { }
    public AttackingLevelOperator(Step step, Player player) : base("Attacking Level Operator", step, player) { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        if(combatEvent.Type == CombatEventType.Attacking)
        {
            combatEvent.Attack.Level = ApplyOperatorFloat(combatEvent.Attack.Level);
        }
    }
}



/*public class Bleeding : Buff, IResolutionHandler
{
    public int LostFractionalHP;//Max 5, and lose one HP;
    public Bleeding(int value) : base("Bleeding", value, true) { }
    public void AfterResolution()
    {
        if (Owner != null)
            FractionalDrain(Owner, Value);
    }
    private void FractionalDrain(Player thisPlayer, int amount)
    {
        LostFractionalHP += amount;
        while (LostFractionalHP >= 5)
        {
            LostFractionalHP -= 5;
            thisPlayer.status.HP.Drain(1);
            PrintEvent.Instance.log += $"{thisPlayer}因为流血失去了1点HP";
        }
    }
}

public class Burning : Buff, IResolutionHandler
{
    public int Burned;
    public Burning(int value) : base("Burning", value, true) { }
    public void AfterResolution()
    {
        if (Owner == null) return;
        Burned += Value;
        PrintEvent.Instance.log += ("现有灼烧" + Value + "\n");

        while (Burned >= 10)
        {
            Burned -= 10;

            int bullet = Owner.status.resources.Bullet.Value;
            int sword = Owner.status.resources.Sword.Value;

            // Case 1: nothing to lose
            if (bullet == 0 && sword == 0)
                return;

            // Case 2: both available → random pick
            if (bullet > 0 && sword > 0)
            {
                int x = UnityEngine.Random.Range(0, 2);
                if (x == 0)
                {
                    Owner.status.resources.Bullet.Lost(1);
                    PrintEvent.Instance.log += $"{Owner.Name} 因灼烧失去了一点子弹";
                }
                else
                {
                    Owner.status.resources.Sword.Lost(1);
                    PrintEvent.Instance.log += $"{Owner.Name} 因灼烧失去了一把剑";
                }
                continue;
            }

            // Case 3: only bullet available
            if (bullet > 0)
            {
                Owner.status.resources.Bullet.Lost(1);
                PrintEvent.Instance.log += $"{Owner.Name} 因灼烧失去了一点子弹";
                continue;
            }

            // Case 4: only sword available
            if (sword > 0)
            {
                Owner.status.resources.Sword.Lost(1);
                PrintEvent.Instance.log += $"{Owner.Name} 因灼烧失去了一把剑";
            }
        }
    }

}

public class Crystallized : Buff, IResolutionHandler
{
    public Crystallized(int value) : base("Crystallized", value, true) { }
    public override void Fade(Player thisPlayer)
    {
        foreach (var action in thisPlayer.action)
        {
            if (action is AttackDefine)
                Value += 1;
            else
                Value -= 1;
        }
        PrintEvent.Instance.log += ($"{Value} 层晶化Now\n");
        if (Value <= 0)
        {
            thisPlayer.status.buffs.Remove(this);
            PrintEvent.Instance.log += ("Remove晶化\n");
        }

    }
    public void AfterResolution()
    {
        if (Owner == null) return;
        int stunDuration = 0;
        while (Value >= 4)
        {
            Value -= 4;
            stunDuration += 2;
        }
        if (stunDuration > 0)
        {
            Owner.status.buffs.Add(new Stunned(stunDuration, null, Owner));
            PrintEvent.Instance.log += ($"晶化爆发，玩家被眩晕{stunDuration}回合\n");
        }
    }
}

public class Cocooned : Buff, IResolutionHandler, IPhaseEnterHandler
{
    public Cocooned(int value) : base("Cocooned", value, false) { }
    public override void Fade(Player thisPlayer)
    {
        Value -= 1;
        if (Value < 0)
            thisPlayer.status.buffs.Remove(this);
    }
    public void AfterResolution()
    {
        if (Owner == null) return;
        if (Value <= 0)
        {
            Owner.status.HP.Set(Owner.status.MaxHP);
            PrintEvent.Instance.log += $"{Owner.Name}重生于茧";
            OnRevive(Owner);
        }
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase && Owner != null)
        {
            foreach (var action in Owner.AvailableActions)
            {
                Owner.ForbiddenActions.Add(action);
            }
        }
    }

    public event Action<Player> OnRevive;

}

//Strength的Buff定义需要完善
public class Strength : Buff, IPhaseExitHandler
{
    public Strength(int value) : base("Strength", value, false) { }
    public void ExitingPhase(Phase phase)
    {
        if (phase is ChasePhase && Owner != null)
        {
            foreach (var attack in Owner.SelectActionType<AttackDefine>())
            {
                attack.Level += Value * 0.5f;
            }
            Debug.Log("Strength" + Value);
        }
    }
}*/