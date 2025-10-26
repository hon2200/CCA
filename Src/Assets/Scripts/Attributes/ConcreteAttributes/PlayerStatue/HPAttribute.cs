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
        Debug.Log("22222222");
        foreach (var skill in victim.hero.skills)
        {
            if (skill is TriggerSkill triggerSkill)
            {
                Debug.Log("33333333333");
                triggerSkill.OnDamaged(attacker, attack.Damage);
            }
        }

        foreach (var skill in attacker.hero.skills)
        {
            if (skill is TriggerSkill triggerSkill)
            {
                Debug.Log("4444444");
                triggerSkill.OnDamaging(attacker,victim,attack.Damage);
            }
        }
        SetValue(Value - amount, "Damage");
    }
    public void Drain(int amount) => SetValue(Value - amount, "Drain");
}
