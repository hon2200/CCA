using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//在某个阶段发动的技能
//写了一个函数，但是也可能不是最优的做法
public abstract class PhasebasedSkill : Skill
{
    protected PhasebasedSkill(string id) : base(id) { }

    // -----------------------------
    // PUBLIC WRAPPERS
    // These shouldn't be virtual, but I need to override them in some skills. 
    // For example, I claim to use this skill in my startPhase, but it only takes effect until the chasePhase
    // In this case, there is a need for two calls of this skill, but OnEvoke can happen only once.
    // -----------------------------
    public virtual void InvokeStartPhase(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer) && OnStartPhase(thisPlayer))
            OnEvoke(thisPlayer);
    }

    public virtual void InvokeAfterSelectingAction(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer) && AfterSelectingAction(thisPlayer))
            OnEvoke(thisPlayer);
    }

    public virtual void InvokeChasePhase(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer) && OnChasePhase(thisPlayer))
            OnEvoke(thisPlayer);
    }

    public virtual void InvokeBeforeResolution(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer) && BeforeResolution(thisPlayer))
            OnEvoke(thisPlayer);
    }

    public virtual void InvokeAfterResolution(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer) && AfterResolution(thisPlayer))
            OnEvoke(thisPlayer);
    }

    public virtual void InvokeEndPhase(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer) && OnEndPhase(thisPlayer))
            OnEvoke(thisPlayer);
    }

    // -----------------------------------------
    // VIRTUAL HOOKS — return true to activate
    // -----------------------------------------
    protected virtual bool OnStartPhase(Player thisPlayer) { return false; }
    protected virtual bool AfterSelectingAction(Player thisPlayer) { return false; }
    protected virtual bool OnChasePhase(Player thisPlayer) { return false; }
    protected virtual bool BeforeResolution(Player thisPlayer) { return false; }
    protected virtual bool AfterResolution(Player thisPlayer) { return false; }
    protected virtual bool OnEndPhase(Player thisPlayer) { return false; }
}
