using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.EventSystems.EventTrigger;


public class NuclearBomb : AttackDefine
{
    public int splash = 1;
    public NuclearBomb()
    {
        ID = "nuclear_bomb";
    }
}


public class WaraxeDance : AttackDefine
{
    public WaraxeDance()
    {
        ID = "waraxe_dance";

        // === Attacking: hit twice + print 三连击
        OnAttackingAction = (attacker, victim) =>
        {
            this.AttackTakeEffect(attacker, victim);
            this.AttackTakeEffect(attacker, victim);

            PrintEvent.Instance.log +=
                $"{attacker.Name}{attacker.ID_inGame} 三连击！";
        };

        // === Overwhelmed: counter → defend → normal attack
        OnOverwhelmedAction = (attacker, enemy) =>
        {
            var counters = this.WatchoutforCounter(enemy);
            var defends = this.WatchoutforDefend(enemy);

            // 1. If countered
            if (counters.Count > 0)
            {
                foreach (var counter in counters)
                {
                    counter.Item1.HowtoCounter(counter.Item2, attacker, enemy, this);

                    // Trigger AttackDefine wrapper → Action
                    OnCountered(attacker, enemy, counter.Item2);
                }
                return;
            }

            // 2. If blocked
            if (defends.Count > 0)
            {
                foreach (var defend in defends)
                {
                    defend.HowtoDefend(this, enemy);
                    OnDefended(attacker, enemy);
                }
                return;
            }

            // 3. Hit normally
            this.AttackTakeEffect(attacker, enemy);
        };
    }
}


