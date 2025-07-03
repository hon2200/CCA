using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RuleCheck
{
    //这个函数负责检查行动（攻击行动的Cost不受目标影响-暂时无法处理激光炮）是否合法
    public static bool isActionLegal(this Player player,ActionDefine actionDefine)
    {
        if (player.status.resources.Bullet.Value - actionDefine.Costs[0] < 0)
            return false;
        if (player.status.resources.AvailableSword.Value - actionDefine.Costs[1] < 0)
            return false;
        //目前过来和挑衅单独判定，之后会把CD对行动合法性的印象单独写出来
        if (actionDefine.Name == "Comeon")
        {
            if (BattleManager.Instance.Turn.Value == 1)
                return false;
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            foreach (var action in actions)
            {
                if (action.ID == "Comeon")
                    return false;
            }
        }
        if (actionDefine.Name == "Provoke")
        {
            if (BattleManager.Instance.Turn.Value == 1)
                return false;
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            foreach (var action in actions)
            {
                if (action.ID == "Provoke")
                    return false;
            }
        }
        return true;
    }

    public static bool isActionFoolish(this Player thisPlayer, ActionDefine actionDefine)
    {
        //如果不是这两类，应该不会出现很蠢的情况
        if(actionDefine is not DefendDefine || actionDefine is not CounterDefine)
        {
            return false;
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            //判断是否被威胁到
            if(ThreatTo(player,thisPlayer,actionDefine))
            {
                //如果是，那么并不愚蠢
                return false;
            }
        }
        //如果不是，那愚蠢至极
        return true;
    }

    //威胁度判断函数
    //某个玩家有能力做出一个或几个攻击行动，以至于需要让你思考是否要做某一个防御行动的威胁
    //举一个栗子，敌人有一发子弹，那他就威胁你考虑单挡，但不会威胁你考虑双挡。
    public static bool ThreatTo(this Player aggressiveOne, Player target, 
        ActionDefine defend)
    {
        var attacks = ActionDataBase.Instance.GetActionType<AttackDefine>();
        foreach(var attack in attacks.Values)
        {
            //如果对手可以做出这样的攻击
            if (isActionLegal(aggressiveOne, attack))
            {
                if (ActionDataBase.Instance.VersusTable.TryGetValue(
                    (attack.ID, defend.ID), out var value))
                {
                    //如果这个防御真的可行
                    if (value != CounterMethod.None)
                    {
                        //威胁论成立！
                        return true;
                    }
                }
                else
                {
                    throw new Exception("Can't find ID in VersusTable" + attack.ID + "and" + defend.ID);
                }
            }
        }
        return false;
    }

}
