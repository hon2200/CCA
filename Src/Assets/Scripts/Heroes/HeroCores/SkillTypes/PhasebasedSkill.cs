using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class PhasebasedSkill : Skill
{
    //在结算阶段前调用
    public virtual void BeforeResolution() { }
}
