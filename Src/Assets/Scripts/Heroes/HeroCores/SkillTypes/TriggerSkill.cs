using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class TriggerSkill : Skill
{
    //受到伤害调用
    public virtual void OnDamaged() { }
    //造成伤害调用
    public virtual void OnDamaging() { }
    //攻击生效调用
    public virtual void OnAttackTakeEffect() { }
    //防御生效调用
    public virtual void OnDefendTakeEffect() { }
}
