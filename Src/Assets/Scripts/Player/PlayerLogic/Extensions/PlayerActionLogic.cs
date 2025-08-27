using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//这个类只装一些Player用于结算行动的方法，拓展功能，避免Player类过于臃肿
public static class PlayerActionLogic
{
    //以下是玩家行动要主要调用的函数
    #region mainFunctions
    public static void Supply(this Player player)
    {
        var supplys = player.SelectActionType<SupplyDefine>();
        foreach (var supply in supplys)
        {
            supply.HowtoSupply();
        }
    }

    public static void Comeon(this Player theComeonOne)
    {
        List<SpecialDefine> specials = theComeonOne.SelectActionType<SpecialDefine>();
        if (theComeonOne.DoYouComeon())
        {
            theComeonOne.HowtoComeon();
        }
    }

    public static void Provoke(this Player player)
    {
        foreach (var provoker in PlayerManager.Instance.Players.Values)
        {
            int turn = BattleManager.Instance.Turn.Value;
            var specials = provoker.SelectActionType_inHistory<SpecialDefine>(turn - 1, true);
            foreach(var special in specials)
            {
                //对方是挑衅，且目标是你
                if (special.ID == "provoke" && special.Target == player.ID_inGame)
                {
                    provoker.OnProvoke(player);
                }
            }
        }
    }

    public static void Attack(this Player player)
    {
        List<AttackDefine> attacks = player.SelectActionType<AttackDefine>();
        foreach (AttackDefine attack in attacks)
        {
            //获取到攻击目标，以Target为键值，找到enemy
            PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player enemy);
            var enemy_attacks = enemy.SelectActionType<AttackDefine>();
            //创建并添加攻击特效
            EffectManager.Instance.CreateTrailEvent("Shoot", player.gameObject, enemy.gameObject);
            //攻击力等级判断
            if (attack.Level > enemy.MaxLevel(player))
            {
                var counters = attack.WatchoutforCounter(enemy);
                var defends = attack.WatchoutforDefend(enemy);
                //对应防御反击判断
                if (counters.Count > 0)
                {
                    foreach (var counter in counters)
                    {
                        counter.Item1.HowtoCounter(counter.Item2, player, enemy, attack);
                    }
                }
                else if (defends.Count > 0)
                {
                    foreach (var defend in defends)
                    {
                        defend.HowtoDefend(attack, enemy);
                    }
                }
                //总算是命中了！
                else
                {
                    attack.HowtoAttack(player, enemy);
                }
            }
            else
            {
            }
        }
    }
    
    public static void CoolDownSword(this Player player)
    {
        bool useSword = false;
        foreach (var action in player.action)
        {
            if (action.Costs[1] != 0)
                useSword = true;
        }
        if (!useSword)
        {
            player.status.resources.AvailableSword.CoolDown(player.status.resources.Sword.Value);
        }
    }

    public static void Consume(this Player player)
    {
        foreach (var action in player.action)
        {
            player.Consume(action);
        }
    }

    public static void Consume(this Player player,ActionDefine action)
    {
        player.status.resources.Bullet.Use(action.Costs[0]);
        player.status.resources.AvailableSword.Use(action.Costs[1]);
    }
    
    public static void RevokeConsume(this Player player, ActionDefine action)
    {
        player.status.resources.Bullet.Get(action.Costs[0]);
        player.status.resources.AvailableSword.Get(action.Costs[1]);
    }

    #endregion

    //以下是一些功能函数
    #region subFunctions
    //一个玩家调用这个函数可以看到他自己某一类型行动的行动列
    //这个选择方法并不依据ActionType，而是直接看行动是不是这个类型
    public static List<Type> SelectActionType<Type>(this Player player) where Type : ActionDefine
    {
        List<Type> list = new();
        foreach (ActionDefine action in player.action)
        {
            if (action is Type typedAction)
            {
                list.Add(typedAction);
            }
        }
        return list;
    }
    //一个玩家调用这个函数可以看到他历史记录里头某一回合某一类型的行动列
    public static List<Type> SelectActionType_inHistory<Type>(this Player player, int Turn, bool isProcessed) where Type : ActionDefine
    {
        if (Turn == 0)
            return new();
        List<Type> list = new();
        player.action.LongHistory.TryGetValue((Turn, isProcessed), out var actionList);
        foreach (ActionDefine action in actionList)
        {
            if (action is Type typedAction)
            {
                list.Add(typedAction);
            }
        }
        return list;
    }
    //调用这个函数可以迅速计算最大攻击力等级
    //如果攻击对象是自身,则不算
    public static float MaxLevel(this Player attacker, Player enemy)
    {
        float maxLevel = 0;
        if (attacker.ID_inGame == enemy.ID_inGame)
            return -10;
        var attacks = enemy.SelectActionType<AttackDefine>();
        foreach (var attack in attacks)
        {
            if (attack.Target == enemy.ID_inGame)
                if (maxLevel < attack.Level)
                    maxLevel = attack.Level;
        }
        return maxLevel;
    }

    //调用这个函数可以判断某人是否进行了过来
    public static bool DoYouComeon(this Player player)
    {
        List<SpecialDefine> specials = player.SelectActionType<SpecialDefine>();
        foreach (SpecialDefine special in specials)
        {
            if (special.ID == "comeon")
                return true;
        }
        return false;
    }
    #endregion
}
