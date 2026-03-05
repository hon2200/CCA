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
    public DamagingOperator(float value, Player player, StepSlot step) : base("Damaging Operator", value, player, step) { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int outcome)
    {
        outcome = ApplyOperatorInt(damage);
    }
}

public class DamagedOperator : BuffOperator, IDamagedHandler
{
    public DamagedOperator(float value, Player player, StepSlot step) : base("Damaged Operator", value, player, step) { }
    public void OnDamaged(Player attacker, Player victim, int damage, out int outcome)
    {
        outcome = ApplyOperatorInt(damage);
    }
}

/// <summary>DamageShield: when victim receives non-zero damage, consume one stack and reduce that damage to 0. Uses IDamagingHandler (runs after Skills and Relics).</summary>
public class DamageShield : Buff, IDamagedHandler
{
    public const string Id = "DamageShield";

    public DamageShield(int value, Player owner) : base(Id, value, false, owner) { }

    public void OnDamaged(Player attacker, Player victim, int damage, out int finalDamage)
    {
        if (damage > 0 && Value > 0)
        {
            Value -= 1;
            finalDamage = 0;
            if (Value <= 0 && Owner != null)
                Owner.status.buffs.Remove(this, "DamageShieldConsumed");
        }
        else
        {
            finalDamage = damage;
        }
    }
}

public class AttackingLevelOperator : BuffOperator, ICombatHandler
{
    public AttackingLevelOperator(float value, Player player, StepSlot step) : base("Attacking Level Operator", value, player, step) { }
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

/// <summary>焚 (BurnMark): +1 DamagingOperator per stack when applied, -1 when lost. Each turn lose one stack in Fade().</summary>
public class BurnMark : Buff
{
    public const string Id = "BurnMark";

    public BurnMark(int value, Player owner) : base(Id, value, true, owner) { }

    public override bool ApplyTo(Buff existing)
    {
        int added = Value;
        base.ApplyTo(existing);
        if (existing is BurnMark bm && added > 0)
            bm.OnStacksApplied(added);
        return true;
    }

    /// <summary>Called when stacks are added (merge or first add). Applies +1 DamagingOperator per stack.</summary>
    public void OnStacksApplied(int count)
    {
        if (Owner?.status?.buffs == null || count <= 0) return;
        for (int i = 0; i < count; i++)
            Owner.status.buffs.Apply(new DamagingOperator(1f, Owner, StepSlot.Third));
    }

    /// <summary>Called when stacks are lost. Applies -1 DamagingOperator per stack.</summary>
    public void OnStackLost(int count)
    {
        if (Owner?.status?.buffs == null || count <= 0) return;
        for (int i = 0; i < count; i++)
            Owner.status.buffs.Apply(new DamagingOperator(-1f, Owner, StepSlot.Third));
    }

    /// <summary>Each turn lose one stack (OnFade). Apply -1 DamagingOperator and remove if no stacks left.</summary>
    public override void Fade()
    {
        if (Value <= 0) return;
        Value--;
        OnStackLost(1);
        if (Value <= 0 && Owner != null)
            Owner.status.buffs.Remove(this, "BurnMarkFade");
    }
}

/// <summary>Poison: each turn drain 1 HP from the owner and decrease Value by 1; removed when Value &lt;= 0.</summary>
public class Poison : Buff
{
    public const string Id = "Poison";

    public Poison(int value, Player owner) : base(Id, value, true, owner) { }

    public override void Fade()
    {
        if (Owner == null || Value <= 0) return;
        Owner.status.HP.Drain(1);
        Value--;
        if (Value <= 0)
            Owner.status.buffs.Remove(this, "PoisonFade");
    }
}

/// <summary>凶 (Omen): each turn the character loses n resources and one 凶 stack, where n = current 凶 stacks. Resources drained from Bullet first, then Sword.</summary>
public class Omen : Buff
{
    public const string Id = "Omen";

    public Omen(int value, Player owner) : base(Id, value, true, owner) { }

    public override void Fade()
    {
        if (Owner == null || Value <= 0) return;
        int n = Value;
        for (int i = 0; i < n; i++)
        {
            int bullet = Owner.status.resources.Bullet.Value;
            int sword = Owner.status.resources.Sword.Value;
            if (bullet > 0)
                Owner.status.resources.Bullet.Lost(1);
            else if (sword > 0)
                Owner.status.resources.Sword.Lost(1);
            else
                break;
        }
        Value--;
        if (Value <= 0)
            Owner.status.buffs.Remove(this, "OmenFade");
    }
}