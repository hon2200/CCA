using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

static class BattleUtil
{
    //一个玩家调用这个函数可以看到他自己某一类型行动的行动列
    public static List<BattleAction<Type>> SelectActionType<Type>(this Player player)where Type:ActionDefine
    {
        List<BattleAction<Type>> list = new ();
        foreach(ActionBase action in player.action)
        {
            if(action is BattleAction<Type> typedAction)
            {
                list.Add(typedAction);
            }
        }
        return list;
    }
    //一个玩家调用这个函数可以看到他历史记录里头某一回合某一类型的行动列
    public static List<BattleAction<Type>> SelectActionType_inHistory<Type>(this Player player,int Turn,bool isProcessed) where Type : ActionDefine
    {
        if (Turn == 0)
            return new();
        List<BattleAction<Type>> list = new();
        player.action.LongHistory.TryGetValue((Turn, isProcessed), out var actionList);
        foreach (ActionBase action in actionList)
        {
            if (action is BattleAction<Type> typedAction)
            {
                list.Add(typedAction);
            }
        }
        return list;
    }
    //调用这个函数可以迅速计算最大攻击力等级
    //如果攻击对象是自身,则不算
    public static float MaxLevel(this Player attacker,Player enemy)
    {
        float maxLevel = 0;
        if (attacker.ID_inGame == enemy.ID_inGame)
            return -10;
        var attacks = enemy.SelectActionType<AttackDefine>();
        foreach(var attack in attacks)
        {
            if (attack.Target == enemy.ID_inGame)
                if (maxLevel < attack.ActionInfo.Level)
                    maxLevel = attack.ActionInfo.Level;
        }
        return maxLevel;
    }

    //调用这两个个函数可以判断一个攻击是否被防御或者反制，返回所有生效的反制类型
    public static List<BattleAction<DefendDefine>> WathoutforDefend( this BattleAction<AttackDefine> attack ,Player enemy)
    {
        List<BattleAction<DefendDefine>> defendMethods = new();
        var enemy_defends = enemy.SelectActionType<DefendDefine>();
        foreach (var defend in enemy_defends)
        {
            if (ActionDataBase.Instance.VersusTable.TryGetValue(
                (attack.ActionInfo.ID, defend.ActionInfo.ID), out var value))
            {
                if (value != CounterMethod.None)
                {
                    defendMethods.Add(defend);
                }
            }
            else
            {
                throw new Exception("Can't find ID in VersusTable");
            }
        }
        return defendMethods;
    }
    
    public static List<(BattleAction<CounterDefine>,CounterMethod)> WathoutforCounter(this BattleAction<AttackDefine> attack, Player enemy)
    {
        List<(BattleAction<CounterDefine>,CounterMethod)> counterMethods = new();
        var enemy_counters = enemy.SelectActionType<CounterDefine>();
        foreach (var counter in enemy_counters)
        {
            if (ActionDataBase.Instance.VersusTable.TryGetValue(
                (attack.ActionInfo.ID, counter.ActionInfo.ID), out var value))
            {
                if (value != CounterMethod.None)
                {
                    counterMethods.Add((counter, value));
                }
            }
            else
            {
                throw new Exception("Can't find ID in VersusTable");
            }
        }
        return counterMethods;
    }
    //调用这个函数可以判断某人是否进行了过来
    public static bool DoYouComeon(this Player player)
    {
        List<BattleAction<SpecialDefine>> specials = player.SelectActionType<SpecialDefine>();
        foreach (BattleAction<SpecialDefine> special in specials)
        {
            if (special.ActionInfo.ID == "comeon")
                return true;
        }
        return false;
    }
}

