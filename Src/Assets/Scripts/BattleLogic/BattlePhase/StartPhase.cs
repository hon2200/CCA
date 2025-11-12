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
        var playersSnapshot = PlayerManager.Instance.Players.Values.ToList();
        //There might be some players created in the process

        foreach (var player in playersSnapshot)
        {
            foreach (var skill in player.hero.skills)
            {
                if (skill is PhasebasedSkill phasebasedSkill)
                {
                    phasebasedSkill.OnStartPhase(player);
                }
            }
        }
        //等待玩家发动技能
        //交互，按钮：问你要不要发动，倒计时
        //如果所有玩家都做好准备
        //进入下一阶段


        //进入开始阶段，不等待玩家（目前的）
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {

    }
}
