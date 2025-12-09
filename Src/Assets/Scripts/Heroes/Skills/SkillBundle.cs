/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//函数or委托？
//创建实例和添加进skillLiberary的方式是否过于粗糙？
//在结算阶段前进行的处理
//猴子的技能
public class MoutainCrusher: PhasebasedSkill
{
    public MoutainCrusher() : base("MountainCrusher") { }
    public override void BeforeResolution(Player thisPlayer)
    {
        foreach(var action in thisPlayer.action)
        {
            if(action is AttackDefine attack)
            {
                attack.Damage *= 3;
            }
        }
    }
}

//吕布的技能
public class LegionBreaker : PhasebasedSkill
{
    public LegionBreaker() : base("LegionBreaker"){ }
    public override void BeforeResolution(Player thisPlayer)
    {
        foreach(var action in thisPlayer.action)
        {
            if(action is AttackDefine attack)
            {
                int n = PlayerManager.Instance.AlivePlayerNumber;
                attack.Level += (0.5f * n);
            }
        }
    }
}

//逃犯比尔
public class DaringBounty : TriggerSkill
{
    public DaringBounty() : base("DaringBounty") { }
    public override void OnDamaged(Player attacker, int damage)
    {
        attacker.status.resources.Bullet.Get(damage);
    }
}



*/