using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StartPhase : Singleton<StartPhase>, Phase
{
    public void OnEnteringPhase()
    {
        BattleManager.Instance.Turn.Advance();
        //进入开始阶段，不等待玩家（目前的）
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {

    }
}
