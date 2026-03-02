using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


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

    public static void Attack(this Player attacker)
    {
        List<AttackDefine> attacks = attacker.SelectActionType<AttackDefine>();
        foreach (AttackDefine attack in attacks)
        {
            //获取到攻击目标，以Target为键值，找到enemy
            PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player enemy);
            // Notify OnAttacking triggers
            CombatDispatcher.Dispatch(new(CombatEventType.Attacking, attacker, enemy, attack), attacker);
            CombatDispatcher.Dispatch(new(CombatEventType.Attacked, attacker, enemy, attack), enemy);
            //创建并添加攻击特效
            EffectManager.Instance.PlayTrailEffect(false, attack.ID, attacker.gameObject, enemy.gameObject);
            //攻击力等级判断
            if (attack.Level > enemy.MaxLevel(attacker))
            {
                attack.HowtoAttack(attacker, enemy);
            }
            else
            {
                attack.OnOverwhelmed(attacker, enemy);
                EffectManager.Instance.PlaySpotEffect(false, "AttackDefend", enemy.gameObject, enemy.MaxLevel(attacker));
                CombatDispatcher.Dispatch(new(CombatEventType.AttackOverwhelmed, attacker, enemy, attack), attacker);
            }
        }
    }
    public static void CoolDown(this Player player)
    {
        player.status.resources.Sword.CoolDown();
        //Deal with CD of actions
        player.CDmanager.CoolDown();
    }

    public static void ConsumeAndCD(this Player player)
    {
        foreach (var action in player.action)
        {
            if (action.isConsumed)
                continue;
            action.isConsumed = true;
            player.Consume(action);
            player.CDmanager.ActionEnterCD(action);
            //挑衅使得所有反弹行动进入CD
            if (action.ID == "provoke")
            {
                var counterDefineList = ActionDataBase.Instance.GetActionType<CounterDefine>().Values;
                foreach(var counter in counterDefineList)
                {
                    player.CDmanager.AddAction(counter.ID, 1);
                }
            }
        }
    }

    public static void Consume(this Player player,ActionDefine action)
    {
        player.status.resources.Bullet.Use(action.Costs[1]);
        player.status.resources.Sword.Use(action.Costs[2]);
    }
    
    public static void RevokeConsume(this Player player, ActionDefine action)
    {
        player.status.resources.Bullet.Use(-action.Costs[1]);
        player.status.resources.Sword.Use(-action.Costs[2]);
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
        List<Type> list = new();
        player.action.LongHistory.TryGetValue((Turn, isProcessed), out var actionList);
        if (actionList == null)
            return new();
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
        var attacks = attacker.SelectActionType<AttackDefine>();
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
