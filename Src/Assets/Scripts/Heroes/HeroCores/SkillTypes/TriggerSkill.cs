using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//触发技，在特定条件下调用
public abstract class TriggerSkill : Skill
{
    //受到伤害调用
    public virtual void OnDamaged(Player attacker, int damage) { }
    //造成伤害调用
    public virtual void OnDamaging() { }
    //攻击生效调用
    public virtual void OnAttackTakeEffect() { }
    //防御生效调用
    public virtual void OnDefendTakeEffect() { }

    //在反弹生效调用......
}
