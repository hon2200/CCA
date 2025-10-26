using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HPAttribute : ObservableAttribute<int>
{
    public void Set(int amount) => SetValue(amount, "Set");
    public void Heal(int amount) => SetValue(Value + amount, "Heal");
    public void Damage(int amount, Player attacker , Player victim, AttackDefine attack)
    {
        foreach (var skill in victim.hero.skills)
        {
            if (skill is TriggerSkill triggerSkill)
            {
                triggerSkill.OnDamaged(attacker, amount);
            }
        }
        foreach (var skill in attacker.hero.skills)
        {
            if (skill is TriggerSkill triggerSkill)
            {
                triggerSkill.OnDamaging(attacker, victim, amount);
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
