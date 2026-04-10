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
    public void Damage(int amount, Player attacker , Player victim, AttackDefine attack)
    {
        PrintEvent.Instance.LogDamage(attacker, victim, amount);
        foreach(var buff in attacker.status.buffs)
        {
            if(buff is IDamagingHandler damageBuff)
            {
                damageBuff.OnDamaging(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach (var skill in attacker.hero.skills)
        {
            if (skill is IDamagingHandler triggerSkill)
            {
                triggerSkill.OnDamaging(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        var relics = RougeManager.Instance?.rougePlayer?.Relics;
        if (relics != null)
        foreach (var relic in relics)
        {
            if (relic is IDamagingHandler triggerRelic)
            {
                triggerRelic.OnDamaging(attacker, victim, amount, out var finalDamage);
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
        if (relics != null)
        foreach(var relic in relics)
        {
            if(relic is IDamagedHandler triggerRelic)
            {
                triggerRelic.OnDamaged(attacker, victim, amount, out int finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        foreach (var buff in victim.status.buffs)
        {
            if (buff is IDamagedHandler damageBuff)
            {
                damageBuff.OnDamaged(attacker, victim, amount, out var finalDamage);
                amount = Mathf.Max(finalDamage, 0);
            }
        }
        SetValue(Value - amount, "Damage");
    }
    public void Drain(int amount) => SetValue(Value - amount, "Drain");
}
