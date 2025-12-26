using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//未来这里需要加入Save&Load，连通ActionPhase里面选择行动的逻辑
public class ChasePhase : Phase
{
    public override void OnEnteringPhase()
    {
    }
    public override void OnExitingPhase()
    {
        //结算消耗
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.ConsumeAndCD();
        }
    }
}

