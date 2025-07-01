using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EndPhase : Singleton<EndPhase>, Phase
{
    public void OnEnteringPhase()
    {
        //进入回合结束阶段，不等待玩家（目前的）
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {
        UpdateHistory();
        ClearMove();
    }
    public void UpdateHistory()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            player.Value.action.ReadinHistory(true);
        }
    }
    public void ClearMove()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            player.Value.action.ClearMove("End");
        }
    }
}
