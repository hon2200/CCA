using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//触发技，在特定条件下调用
public abstract class TriggerSkill : Skill
{
    protected TriggerSkill(string id) : base(id) { }

    // -----------------------------
    // PUBLIC INVOKERS
    // -----------------------------
    public void InvokeOnAttacked(Player attacker, Player victim, AttackDefine attack)
    {
        if (IsAvailable(victim) && OnAttacked(attacker, victim, attack))
            OnEvoke(attacker);
    }

    public void InvokeOnDamaged(Player attacker, Player victim, int damage)
    {
        if (IsAvailable(victim) && OnDamaged(attacker, victim, damage)) 
            OnEvoke(attacker);
    }

    public void InvokeOnDamaging(Player attacker, Player victim, int damage)
    {
        if (IsAvailable(attacker) && OnDamaging(attacker, victim, damage))
            OnEvoke(attacker);
    }

    public void InvokeOnAttackOverwhelmed(Player attacker, Player victim, AttackDefine attack)
    {
        if (IsAvailable(victim) && OnAttackOverwhelmed(attacker, victim, attack))
            OnEvoke(attacker);
    }

    public void InvokeOnAttackTakeEffect(Player attacker, Player victim, AttackDefine attack)
    {
        if (IsAvailable(attacker) && OnAttackTakeEffect(attacker, victim, attack))
            OnEvoke(attacker);
    }

    public void InvokeOnDefendTakeEffect(Player self)
    {
        if (IsAvailable(self) && OnDefendTakeEffect(self))
            OnEvoke(self);
    }

    public void InvokeOnCounterTakeEffect(Player self)
    {
        if (IsAvailable(self) && OnCounterTakeEffect(self))
            OnEvoke(self);
    }

    public bool InvokeOnDeath(Player self)
    {
        if (IsAvailable(self) && OnDeath(self, out bool revive))
        {
            OnEvoke(self);
            if (revive)
                self.status.life.Revive();
            return true;
        }
        return false;
    }

    // ---------------------------------
    // PROTECTED VIRTUAL TRIGGER METHODS
    // return true → trigger the skill
    // ---------------------------------

    protected virtual bool OnAttacked(Player attacker, Player victim, AttackDefine attack)
    { return false; }

    protected virtual bool OnDamaged(Player attacker, Player victim, int damage)
    { return false; }

    protected virtual bool OnDamaging(Player attacker, Player victim, int damage)
    { return false; }

    protected virtual bool OnAttackOverwhelmed(Player attacker, Player vitim, AttackDefine attack)
    { return false; }

    protected virtual bool OnAttackTakeEffect(Player attacker, Player victim, AttackDefine attack)
    { return false; }

    protected virtual bool OnDefendTakeEffect(Player self)
    { return false; }

    protected virtual bool OnCounterTakeEffect(Player self)
    { return false; }

    // If this skill is evoked => return true;
    // If the player is revived from this skill => Revive is true, only check this after returning true;
    protected virtual bool OnDeath(Player self, out bool Revive)
    { Revive = false; return false; }
}
