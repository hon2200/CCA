using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//回合结算阶段脚本
public class ResolutionPhase : Singleton<ResolutionPhase>
{
    //结算阶段，结算大家的行动
    //由于目前还没引入各种buff，所以结算阶段就先写在这里
    public void Resolution()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            Consume(player);
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            Provoke(player);
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            Comeon(player);
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            Supply(player);
            Attack(player);
        }
        PrintResult_Debug();
    }
    public void PrintResult_Debug()
    {
        if (BattleManager.Instance.Turn.Value == 1)
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players, 
                new string[] {"ID_inGame", "status"},"Log/InGame/PlayerStatus.txt");
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action.Value", "Log/InGame/PlayerAction.txt");
        }
        else
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players,
                new string[] { "ID_inGame", "status" }, "Log/InGame/PlayerStatus.txt", false);
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action.Value", "Log/InGame/PlayerAction.txt",false);
        }

    }
    private void Supply(Player player)
    {
        var supplys = player.SelectActionType<SupplyDefine>();
        foreach (var supply in supplys)
        {
            PlayerManager.Instance.Players.TryGetValue(supply.Target, out Player receiver);
            //理论上这里也可以把resource改成一个list<int>，但是考虑到resource里面还有swordinCD，所以就不改了
            receiver.status.resources.Bullet.Get(supply.ActionInfo.SupplyNumber[0]);
            receiver.status.resources.Sword.Get(supply.ActionInfo.SupplyNumber[1]);
            receiver.status.resources.AvailableSword.Get(supply.ActionInfo.SupplyNumber[1]);
        }
    }
    private void Comeon(Player theComeonOne)
    {
        List<BattleAction<SpecialDefine>> specials = theComeonOne.SelectActionType<SpecialDefine>();
        if(theComeonOne.DoYouComeon())
            foreach(var theBeComeonedOne in PlayerManager.Instance.Players)
            {
                var attacks = theBeComeonedOne.Value.SelectActionType<AttackDefine>();
                var supplys = theBeComeonedOne.Value.SelectActionType<SupplyDefine>();
                foreach(var attack in attacks)
                {
                    PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player originalTarget);
                    //如果这个行动已经是过来的复制体了，那么多次计算会造成重复
                    if (attack.ActionInfo.isCopy)
                        continue;
                    //如果敌人正在使用过来，则多个过来会创造撕裂
                    //如果这个攻击本身就是对自己的攻击，那么直接跳过这一条
                    if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != theComeonOne.ID_inGame)
                    {
                        var attackcopy = (BattleAction<AttackDefine>)attack.Clone();
                        attackcopy.Target = theComeonOne.ID_inGame;
                        attackcopy.ActionInfo.isCopy = true;
                        theBeComeonedOne.Value.action.Add(attackcopy, "InGame");
                    }
                    //过来直接改变目标
                    else
                    {
                        attack.Target = theComeonOne.ID_inGame;
                    }
                }
                foreach(var supply in supplys)
                {
                    PlayerManager.Instance.Players.TryGetValue(supply.Target, out Player originalTarget);
                    //如果这个行动已经是过来的复制体了，那么多次计算会造成重复
                    if (supply.ActionInfo.isCopy)
                        continue;
                    //如果敌人正在使用过来，则多个过来会创造撕裂
                    if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != theComeonOne.ID_inGame)
                    {
                        var supplycopy = (BattleAction<SupplyDefine>)supply.Clone();
                        supplycopy.Target = theComeonOne.ID_inGame;
                        supplycopy.ActionInfo.isCopy = true;
                        theBeComeonedOne.Value.action.Add(supplycopy, "InGame");
                    }
                    //过来直接改变目标
                    else
                    {
                        supply.Target = theComeonOne.ID_inGame;
                    }
                }    
            }
    }
    private void Provoke(Player player)
    {
        List<BattleAction<SpecialDefine>> specials = player.SelectActionType_inHistory<SpecialDefine>(BattleManager.Instance.Turn.Value - 1, true);
        foreach (BattleAction<SpecialDefine> special in specials)
        {
            //是挑衅
            if (special.ActionInfo.ID == "provoke")
            {
                PlayerManager.Instance.Players.TryGetValue(special.Target, out Player enemy);
                special.ActionInfo.OnProvoke(player, enemy);
            }
        }
    }
    private void Attack(Player player)
    {
        List<BattleAction<AttackDefine>> attacks = player.SelectActionType<AttackDefine>();
        foreach(BattleAction<AttackDefine> attack in attacks)
        {
            //获取到攻击目标，以Target为键值，找到enemy
            PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player enemy);
            var enemy_attacks = enemy.SelectActionType<AttackDefine>();
            //攻击力等级判断
            if (attack.ActionInfo.Level > enemy.MaxLevel(player))
            {
                var counters = attack.WathoutforCounter(enemy);
                var defends = attack.WathoutforDefend(enemy);
                //对应防御反击判断
                if (counters.Count > 0)
                {
                    foreach (var counter in counters)
                    {
                        counter.Item1.ActionInfo.HowtoCounter(counter.Item2, player, enemy, attack);
                    }
                }
                else if (defends.Count > 0)
                {
                    foreach(var defend in defends)
                    {
                        defend.ActionInfo.HowtoDefend(attack);
                    }
                }
                //总算是命中了！
                else
                {
                    attack.ActionInfo.HowtoAttack(player, enemy, attack);
                }
            }
            else
            {
                attack.isEffective = false;
            }
        }
    }
    //结算玩家资源的消耗以及剑的回复
    private void Consume(Player player)
    {
        bool useSword = false;
        foreach(var action in player.action)
        {
            player.status.resources.Bullet.Use(action.ActionInfo.Costs[0]);
            player.status.resources.AvailableSword.Use(action.ActionInfo.Costs[1]);
            if (action.ActionInfo.Costs[1] !=0) 
                useSword = true;
        }
        if(!useSword)
        {
            player.status.resources.AvailableSword.CoolDown(player.status.resources.Sword.Value);
        }
    }
}
