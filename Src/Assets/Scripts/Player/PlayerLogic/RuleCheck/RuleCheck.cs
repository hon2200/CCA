using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class RuleCheck
{
    //这个函数负责检查行动是否合法
    public static bool isActionLegal(this Player player,ActionDefine actionDefine)
    {
        //已经有超过一个行动了，多重行动合法性检查
        if (player.action.Count >= 1)
        {
            foreach(var action in player.action)
            {
                //如果有对自己的行动，那么无法进行其他行动
                if (action.TargetType == TargetType.Self)
                    return false;
                //如果是攻击行动，那么如果攻击目标重复则不行
                if (action.TargetType == TargetType.Enemy)
                    if (action.Target == actionDefine.Target)
                        return false;
            }
        }
        //攻击行动检查，不能打死人和自己，不能打队友
        if (actionDefine.TargetType == TargetType.Enemy)
        {
            PlayerManager.Instance.Players.TryGetValue(actionDefine.Target, out var targerPlayer);
            //当然，如果这个攻击行动还没有选择对象，那其实没有关系
            if (targerPlayer != null)
            {
                if (actionDefine.Target == player.ID_inGame || targerPlayer.status.life.Value == LifeStatus.Death)
                    return false;
                //目前只有一个粗糙的组队检测方法
                if (targerPlayer is AIPlayer aiTargetPlayer)
                {
                    if(aiTargetPlayer.isFriend)
                    {
                        if (player.playerType == PlayerType.Human)
                            return false;
                        if (player is AIPlayer aiPlayer && aiPlayer.isFriend)
                            return false;
                    }
                    if(!aiTargetPlayer.isFriend)
                    {
                        if (player is AIPlayer aiPlayer && !aiPlayer.isFriend)
                            return false;
                    }
                }
                if(targerPlayer.playerType == PlayerType.Human)
                {
                    if (player.playerType == PlayerType.Human)
                        return false;
                    if (player is AIPlayer aiPlayer && aiPlayer.isFriend)
                        return false;
                }
            }

        }


        //资源消耗检查
        if (player.status.resources.Bullet.Value - actionDefine.Costs[0] < 0)
            return false;
        if (player.status.resources.AvailableSword.Value - actionDefine.Costs[1] < 0)
            return false;
        //目前过来和挑衅单独判定，之后会把CD对行动合法性的印象单独写出来
        if (actionDefine.ID == "comeon")
        {
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            //此前没有行动，说明刚上场
            if (actions == null)
                return false;
            foreach (var action in actions)
            {
                if (action.ID == "comeon")
                    return false;
            }
        }
        if (actionDefine.ID == "provoke")
        {
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            //此前没有行动，说明刚上场
            if (actions == null)
                return false;
            foreach (var action in actions)
            {
                if (action.ID == "provoke" && action.Target == actionDefine.Target)
                    return false;
            }
        }
        //激光炮也单独判定，因为激光炮的赋值在PreResolution，which在Action之后
        if(actionDefine.ID == "laser_cannon")
        {
            if (player.status.resources.Bullet.Value < 2)
                return false;
            PlayerManager.Instance.Players.TryGetValue(actionDefine.Target, out var victim);
            if(victim != null)
                if (player.status.resources.Bullet.Value < victim.status.HP.Value - 1)
                    return false;
        }
        return true;
    }
    public static bool isActionFoolish(this Player thisPlayer, ActionDefine actionDefine)
    {
        //如果不是这两类，应该不会出现很蠢的情况
        if(actionDefine is not DefendDefine && actionDefine is not CounterDefine)
        {
            return false;
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            // 跳过自己和无生命的玩家
            if (player == thisPlayer || player.status.life.Value == LifeStatus.Death)
                continue;

            // 判断该玩家是否对当前玩家构成威胁，需要这种防御/反击
            if (ThreatTo(player,thisPlayer,actionDefine))
            {
                //如果是，那么并不愚蠢
                return false;
            }
        }
        //如果不是，那愚蠢至极
        return true;
    }
    public static bool isActionAvailable(this Player thisPlayer,ActionDefine actionDefine)
    {
        if(thisPlayer.AvailableActions.Count==0)
        {
            Debug.Log("None in AvailableActions Yet, Want to fix it?");
            return true;
        }
        else
        {
            if (thisPlayer.AvailableActions.Contains(actionDefine.ID))
                return true;
            return false;
        }
    }

    //威胁度判断函数
    //某个玩家有能力做出一个或几个攻击行动，以至于需要让你思考是否要做某一个防御行动的威胁
    //举一个栗子，敌人有一发子弹，那他就威胁你考虑单挡，但不会威胁你考虑双挡。
    private static bool ThreatTo(this Player aggressiveOne, Player target, 
        ActionDefine defend)
    {
        var attacks = ActionDataBase.Instance.GetActionType<AttackDefine>();
        foreach(var attack in attacks.Values)
        {
            //如果对手可以做出这样的攻击
            if (isActionLegal(aggressiveOne, attack) && isActionAvailable(aggressiveOne, attack))
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
