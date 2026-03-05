using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

//Delegates do not guarantee order unless you manually manage invocation lists, which becomes messy.
//So, in the things related to game core logic, don't use delegation.
public class HPAttribute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void Heal(int amount) => SetValue(Value + amount, "Heal");
    public void Damage(int amount, Player attacker, Player victim, AttackDefine attack)
    {
        PrintEvent.Instance.LogDamage(attacker, victim, amount);
        foreach (var skill in attacker.hero.skills)
        {
            if (skill is IDamagingHandler triggerSkill)
            {
                triggerSkill.OnDamaging(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach (var relic in RougeManager.Instance.rougePlayer.Relics)
        {
            if (relic is IDamagingHandler triggerRelic)
            {
                triggerRelic.OnDamaging(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach (var buff in attacker.status.buffs)
        {
            if (buff is IDamagingHandler triggerBuff)
            {
                triggerBuff.OnDamaging(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach (var skill in victim.hero.skills)
        {
            if (skill is IDamagedHandler triggerSkill)
            {
                triggerSkill.OnDamaged(attacker, victim, amount, out int finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach(var relic in RougeManager.Instance.rougePlayer.Relics)
        {
            if(relic is IDamagedHandler triggerRelic)
            {
                triggerRelic.OnDamaged(attacker, victim, amount, out int finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach (var buff in victim.status.buffs)
        {
            if (buff is IDamagedHandler triggerBuff)
            {
                triggerBuff.OnDamaged(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        if (attacker is AIPlayer aiAttacker)
        {
            aiAttacker.DamagingReaction(amount);
        }
        if(victim is AIPlayer aiVictim)
        {
            aiVictim.DamagedReaction(amount);
        }
        SetValue(Value - amount, "Damage");
    }
    public void Drain(int amount) => SetValue(Value - amount, "Drain");
}
