using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EndPhase : Singleton<EndPhase>
{
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
            player.Value.action.ClearMove();
        }
    }
}
