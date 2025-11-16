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
    }

    //附加命中两次
    public override void OnAttacking(Player attacker, Player victim)
    {
        this.HowtoAttack(attacker, victim);
        this.HowtoAttack(attacker, victim);
        PrintEvent.Instance.log += (attacker.Name + attacker.ID_inGame + "三连击！");
    }
    //命中一次
    public override void OnOverwhelmed(Player attacker, Player enemy)
    {
        var counters = this.WatchoutforCounter(enemy);
        var defends = this.WatchoutforDefend(enemy);
        //对应防御反击判断
        if (counters.Count > 0)
        {
            foreach (var counter in counters)
            {
                counter.Item1.HowtoCounter(counter.Item2, attacker, enemy, this);
                this.OnCountered(attacker, enemy, counter.Item2);
            }
        }
        else if (defends.Count > 0)
        {
            foreach (var defend in defends)
            {
                defend.HowtoDefend(this, enemy);
                this.OnDefended(attacker, enemy);
            }
        }
        //总算是命中了！
        else
        {
            this.HowtoAttack(attacker, enemy);
        }
    }
}

