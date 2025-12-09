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
        foreach (var skill in victim.hero.skills)
        {
            if (skill is TriggerSkill triggerSkill)
            {
                triggerSkill.InvokeOnDamaged(attacker, victim, amount);
            }
        }
        foreach (var skill in attacker.hero.skills)
        {
            if (skill is TriggerSkill triggerSkill)
            {
                triggerSkill.InvokeOnDamaging(attacker, victim, amount);
            }
        }
        if(attacker is AIPlayer aiAttacker)
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
