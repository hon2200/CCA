using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//在某个阶段发动的技能
//写了一个函数，但是也可能不是最优的做法
public abstract class PhasebasedSkill : Skill
{
    // 在开始阶段调用
    public virtual void OnStartPhase(Player thisPlayer) { }

    // 在回合或阶段结束时调用
    public virtual void OnPhaseEnd(Player thisPlayer) { }

    // 在结算阶段前调用
    public virtual void BeforeResolution(Player thisPlayer) { }

    // 在结算阶段后调用
    public virtual void AfterResolution(Player thisPlayer) { }
}
