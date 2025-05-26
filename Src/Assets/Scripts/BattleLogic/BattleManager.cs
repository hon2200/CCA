using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BattleManager: MonoSingleton<BattleManager>
{
    public TurnAttribute Turn;
    public void Start()
    {
        Turn = new();
    }
    public void Round()
    {
        //回合开始阶段
        Turn.Advance();
        //无事发生

        //行动阶段
        ActionPhase.Instance.ReadinOnlyPlayerActs_Debug();
        //结算阶段
        ResolutionPhase.Instance.Resolution();
        //回合结束阶段
        EndPhase.Instance.UpdateHistory();
        EndPhase.Instance.ClearMove();
    }
}