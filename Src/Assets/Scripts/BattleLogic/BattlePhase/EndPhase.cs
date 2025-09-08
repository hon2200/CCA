using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EndPhase : Singleton<EndPhase>, Phase
{
    public void OnEnteringPhase()
    {
        ChangeEmotionAndHonesty();
        //进入回合结束阶段，不等待玩家（目前的）
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {
        UpdateHistory();
        ClearMove();
    }
    private void UpdateHistory()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            player.Value.action.ReadinHistory(true);
        }
    }
    private void ClearMove()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            player.Value.action.ClearMove("End");
        }
    }
    //改变一下所有人机玩家的
    public void ChangeEmotionAndHonesty()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            if(player is AIPlayer AI)
            {
                AI.TurnBasedChange();
            }
        }
    }
}
