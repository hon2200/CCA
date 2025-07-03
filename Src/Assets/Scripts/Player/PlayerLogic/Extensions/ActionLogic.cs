using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public static class AttackLogic
{
    //假设攻击生效后调用这个函数
    public static void HowtoAttack(this AttackDefine attack ,Player attacker, Player victim)
    {
        switch (attack.ID)
        {
            //溅射伤害，暂时没有通过buff去定义它，之后想到好的架构再去加，先暂时写在这里
            case "nuclear_bomb":
                victim.status.HP.Damage(attack.Damage);
                foreach (var player in PlayerManager.Instance.Players)
                {
                    if (player.Key != victim.ID_inGame)
                    {
                        player.Value.status.HP.Damage(1);
                    }
                }
                break;
            default:
                victim.status.HP.Damage(attack.Damage);
                break;
        }
        attack.isEffective = true;
    }
    //调用这两个个函数可以判断一个攻击是否被防御或者反制，返回所有生效的反制类型
    public static List<DefendDefine> WathoutforDefend( this AttackDefine attack ,Player enemy)
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
    
    public static List<(CounterDefine,CounterMethod)> WathoutforCounter(this AttackDefine attack, Player enemy)
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
    public static void HowtoDefend(this DefendDefine defend,AttackDefine attack)
    {
        attack.isEffective = false;
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
                attack.isEffective = false;
                break;
            case CounterMethod.Disarm:
                attack.isEffective = false;
                attacker.status.resources.AvailableSword.Set(0);
                break;
            case CounterMethod.Rebounce:
                attack.isEffective = true;
                attack.HowtoAttack(attacker, attacker);
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
        //理论上这里也可以把resource改成一个list<int>，但是考虑到resource里面还有swordinCD，所以就不改了
        receiver.status.resources.Bullet.Get(supply.SupplyNumber[0]);
        receiver.status.resources.Sword.Get(supply.SupplyNumber[1]);
        receiver.status.resources.AvailableSword.Get(supply.SupplyNumber[1]);
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
            victim.status.HP.Drain(1);
    }

    public static void HowtoComeon(this Player Comeoner)
    {
        foreach (var theBeComeonedOne in PlayerManager.Instance.Players)
        {
            var attacks = theBeComeonedOne.Value.SelectActionType<AttackDefine>();
            var supplys = theBeComeonedOne.Value.SelectActionType<SupplyDefine>();
            foreach (var attack in attacks)
            {
                PlayerManager.Instance.Players.TryGetValue(attack.Target, out Player originalTarget);
                //如果这个行动已经是过来的复制体了，那么多次计算会造成重复
                if (attack.isCopy)
                    continue;
                //如果敌人正在使用过来，则多个过来会创造撕裂
                //如果这个攻击本身就是对自己的攻击，那么直接跳过这一条
                if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != Comeoner.ID_inGame)
                {
                    var attackcopy = (AttackDefine)attack.Clone();
                    attackcopy.Target = Comeoner.ID_inGame;
                    attackcopy.isCopy = true;
                    theBeComeonedOne.Value.action.Add(attackcopy, "InGame");
                }
                //过来直接改变目标
                else
                {
                    attack.Target = Comeoner.ID_inGame;
                }
            }
            foreach (var supply in supplys)
            {
                PlayerManager.Instance.Players.TryGetValue(supply.Target, out Player originalTarget);
                //如果这个行动已经是过来的复制体了，那么多次计算会造成重复
                if (supply.isCopy)
                    continue;
                //如果敌人正在使用过来，则多个过来会创造撕裂
                if (originalTarget.DoYouComeon() && originalTarget.ID_inGame != Comeoner.ID_inGame)
                {
                    var supplycopy = (SupplyDefine)supply.Clone();
                    supplycopy.Target = Comeoner.ID_inGame;
                    supplycopy.isCopy = true;
                    theBeComeonedOne.Value.action.Add(supplycopy, "InGame");
                }
                //过来直接改变目标
                else
                {
                    supply.Target = Comeoner.ID_inGame;
                }
            }
        }
    }
}
