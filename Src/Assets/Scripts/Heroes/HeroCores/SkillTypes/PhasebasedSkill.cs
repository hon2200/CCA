using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//在某个阶段发动的技能
//写了一个函数，但是也可能不是最优的做法
public abstract class PhasebasedSkill : Skill
{
    //在结算阶段前调用
    public virtual void BeforeResolution() { }
}
