using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class AttackLogic
{
    //假设攻击生效后调用这个函数
    public static void HowtoAttack(this AttackDefine attack ,Player attacker, Player victim, Player rebouncer = null)
    {
        victim.status.HP.Damage(attack.Damage, attacker, victim, attack);
        attack.Victim = victim.ID_inGame;
        if (rebouncer != null && !victim.possibleKillers.Contains(rebouncer.ID_inGame))
            victim.possibleKillers.Add(rebouncer.ID_inGame);
        else if(!victim.possibleKillers.Contains(attacker.ID_inGame))
            victim.possibleKillers.Add(attacker.ID_inGame);
    }
    //调用这两个个函数可以判断一个攻击是否被防御或者反制，返回所有生效的反制类型
    public static List<DefendDefine> WatchoutforDefend( this AttackDefine attack ,Player enemy)
    {
        List<DefendDefine> defendMethods = new();
        var enemy_defends = enemy.SelectActionType<DefendDefine>();
        foreach (var defend in enemy_defends)
        {
            if (ActionDataBase.Instance.VersusTable.TryGetValue(
                (attack.ID, defend.ID), out var value))
            {
                if (value != CounterMethod.None)
                {
                    defendMethods.Add(defend);
                }
            }
            else
            {
                throw new Exception("Can't find ID in VersusTable" + attack.ID + "and" + defend.ID);
            }
        }
        return defendMethods;
    }
    
    public static List<(CounterDefine,CounterMethod)> WatchoutforCounter(this AttackDefine attack, Player enemy)
    {
        List<(CounterDefine,CounterMethod)> counterMethods = new();
        var enemy_counters = enemy.SelectActionType<CounterDefine>();
        foreach (var counter in enemy_counters)
        {
            if (ActionDataBase.Instance.VersusTable.TryGetValue(
                (attack.ID, counter.ID), out var value))
            {
                if (value != CounterMethod.None)
                {
                    counterMethods.Add((counter, value));
                }
            }
            else
            {
                throw new Exception("Can't find ID in VersusTable" + attack.ID + "and" + counter.ID);
            }
        }
        return counterMethods;
    }
}

public static class DefendLogic
{
    //目前防御没有任何额外效果
    public static void HowtoDefend(this DefendDefine defend,AttackDefine attack, Player victim)
    {
        //创建并添加防御
        EffectManager.Instance.PlaySpotEffect(false, "Shield", victim.gameObject);
    }
}

public static class CounterLogic
{
    public static void HowtoCounter(this CounterDefine counter, 
        CounterMethod counterType, Player attacker, Player victim, AttackDefine attack)
    {
        switch (counterType)
        {
            case CounterMethod.Block:
                //创建并添加防御
                EffectManager.Instance.PlaySpotEffect(false,"Shield", victim.gameObject);
                break;
            case CounterMethod.Disarm:
                //创建并添加防御
                EffectManager.Instance.PlaySpotEffect(false, "Shield", victim.gameObject);
                attacker.status.resources.Sword.ForcedCD(attacker.status.resources.Sword.Value);
                break;
            case CounterMethod.Rebounce:
                //创建并添加防御
                EffectManager.Instance.PlaySpotEffect(false, "Shield", victim.gameObject);
                //创建并添加反击路线
                EffectManager.Instance.PlayTrailEffect(false, "Bullet", victim.gameObject, attacker.gameObject);
                attack.HowtoAttack(attacker, attacker, victim);
                break;
            default:
                throw new Exception("Wrong Counter Type");
        }
    }

}

public static class SupplyLogic
{
    public static void HowtoSupply(this SupplyDefine supply)
    {
        PlayerManager.Instance.Players.TryGetValue(supply.Target, out Player receiver);
        if(receiver != null)
        {
            //理论上这里也可以把resource改成一个list<int>，但是考虑到resource里面还有swordinCD，所以就不改了
            receiver.status.HP.Heal(supply.SupplyNumber[0]);
            receiver.status.resources.Bullet.Get(supply.SupplyNumber[1]);
            receiver.status.resources.Sword.Get(supply.SupplyNumber[2]);
        }    
        else
        {
            Debug.Assert(false, "Can't find Supply Receiver"+ supply.Target);
        }
    }
}

public static class SpecialLogic
{
    private static bool RespondingProvoke(this Player provoker, Player victim)
    {
        var attacks = victim.SelectActionType<AttackDefine>();
        foreach (var attack in attacks)
        {
            if (attack.Target == provoker.ID_inGame)
                return true;
        }
        return false;
    }
    public static void OnProvoke(this Player provoker, Player victim)
    {
        if (!RespondingProvoke(provoker, victim))
        {
            victim.status.HP.Drain(1);
            PrintEvent.Instance.LogProvoke(victim, provoker, 1);
        }
    }
    public static void HowtoComeon(this Player Comeoner)
    {
        RedirectActions<AttackDefine>(Comeoner);
        RedirectActions<SupplyDefine>(Comeoner);
    }
    private static void RedirectActions<T>(Player Comeoner) where T : ActionDefine
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            var actions = player.SelectActionType<T>();
            foreach (var action in actions)
            {
                PlayerManager.Instance.Players.TryGetValue(action.Target, out Player originalTarget);
                if (action.isCopy)
                    continue;

                PrintEvent.Instance.LogComeon(Comeoner, player, action);
                if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != Comeoner.ID_inGame)
                {
                    var copy = (T)action.Clone();
                    copy.Target = Comeoner.ID_inGame;
                    copy.isCopy = true;
                    player.action.Add(copy, "InGame");
                }
                else
                {
                    action.Target = Comeoner.ID_inGame;
                }

            }
        }
    }
}

public static class PreCookLogic
{
    public static void GiveValueToLaserCannon(this Player player)
    {
        foreach (var attack in player.SelectActionType<AttackDefine>())
        {
            if (attack.ID == "laser_cannon")
                attack.LaserCannon();
        }
    }
}