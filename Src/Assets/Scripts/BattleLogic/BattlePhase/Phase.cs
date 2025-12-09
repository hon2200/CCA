using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//管理各个阶段
public interface Phase
{
    public abstract void OnEnteringPhase();
    public abstract void OnExitingPhase();
}


public enum PhaseName
{
    StartPhase = 1,
    ActionPhase = 2,
    ChasePhase = 3,
    PreResolutionPhase = 4,
    ResolutionPhase = 5,
    EndPhase = 6,
}