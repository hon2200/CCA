using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StartPhase : Phase
{
    public override void OnEnteringPhase()
    {
        PrintEvent.Instance.log += $"\n进入下一回合\n";
        BattleManager.Instance.Turn.Advance();
        //刷新剑的使用情况
        SwordUseRefresh();
        //刷新不可用行动
        ForbiddenActionRefresh();
        //刷新技能
        RefreshSkills();
        //等待玩家发动技能
        //交互，按钮：问你要不要发动，倒计时
        //如果所有玩家都做好准备
        //进入下一阶段
    }
    public override void OnExitingPhase()
    {
    }

    private void SwordUseRefresh()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.status.resources.Sword.OnNewTurn();
        }
    }

    private void ForbiddenActionRefresh()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.ForbiddenActions.Clear();
        }
    }
    private void RefreshSkills()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            foreach(var skill in player.hero.skills)
            {
                skill.CDCountDown();
            }
        }
    }
}
