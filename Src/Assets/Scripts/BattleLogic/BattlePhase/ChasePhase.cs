using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//未来这里需要加入Save&Load，连通ActionPhase里面选择行动的逻辑
public class ChasePhase : Singleton<ChasePhase>, Phase
{
    public void OnEnteringPhase()
    {
        var playersSnapshot = PlayerManager.Instance.Players.Values.ToList();
        //There might be some players created in the process

        foreach (var player in playersSnapshot)
        {
            foreach (var skill in player.hero.skills)
            {
                if (skill is PhasebasedSkill phasebasedSkill)
                {
                    phasebasedSkill.InvokeChasePhase(player);
                }
            }
        }

        //目前没啥好等玩家的，直接下一阶段。
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {
        //结算消耗
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.ConsumeAndCD();
        }
    }
}

