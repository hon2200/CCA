using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EndPhase : Phase
{
    public override void OnEnteringPhase()
    {
        ChangeEmotionAndHonesty();
    }
    public override void OnExitingPhase()
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
