using System;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;

//行动类别
[Serializable]
public enum CardType
{
    Supply = 1,
    BulletAttack = 2, 
    SwordAttack = 3,
    Defend = 4,
    Counter = 5,
    Special = 6,
}

public enum ActionType
{
    Origin = 0,
    Supply = 1,
    Attack = 2,
    Defend = 3,
    Counter = 4,
    Special = 5,
}


//目标的类别
public enum TargetType
{
    Self = 0,  //默认目标是自己，如补给，防御，反制，过来
    Enemy = 1, //默认目标是敌人，如攻击和挑衅
}
    
//补给or消耗资源的类别
public enum SupplyType
{
    Bullet = 1,
    Sword = 2,
}

//攻击类型
public enum AttackType
{
    BulletAttack = 1,
    SwordAttack = 2,
}

//反制类别
public enum CounterMethod
{
    None = 0,
    Block = 1,
    Rebounce = 2,
    Disarm = 3,
}

//父类，行动基础信息
public class ActionDefine : ICloneable
{
    public string ID { get; set; } // 行动ID
    public string Name { get; set; } // 行动名称
    public string Description { get; set; } // 行动描述
    public List<int> Costs { get; set; } // 行动消耗
    public int CD { get; set; } //冷却时间
    public int remainedCD { get; set; }
    public TargetType TargetType { get; set; } // 目标类型//未来可能在读取行动上面有用
    public int Target { get; set; } //目标
    public bool isCopy { get; set; } //是否是“过来”产生的复制体，等价于：是否响应过来
    public bool isBasic { get; set; } //是否是每一个玩家都可以拥有的基础行动
    public ActionType actionType { get => GetActionType(); }
    public virtual void Copy(ActionDefine target)
    {
        var type = GetType(); // e.g. NuclearBomb, not just ActionDefine

        // Get all instance properties (public and non-public) that can be written to
        var props = type.GetProperties(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        foreach (var prop in props)
        {
            if (!prop.CanWrite) continue; // skip read-only
            var value = prop.GetValue(this);

            if (value == null)
            {
                prop.SetValue(target, null);
                continue;
            }

            if (value is System.Collections.IList list)
            {
                // Create a new list of the same type and copy elements
                var listType = list.GetType();
                var newList = (System.Collections.IList)Activator.CreateInstance(listType);
                foreach (var item in list)
                    newList.Add(item);
                prop.SetValue(target, newList);
            }
            else
            {
                // Shallow copy for primitives, strings, etc.
                prop.SetValue(target, value);
            }
        }
    }
    // 实现 ICloneable 接口（深拷贝）
    public virtual object Clone()
    {
        // Use runtime type constructor instead of hardcoded ActionDefine
        var type = GetType();
        var clone = (ActionDefine)Activator.CreateInstance(type);
        Copy(clone);
        return clone;
    }
    public virtual ActionType GetActionType() { return ActionType.Origin; }
}

//补给类行动
public class SupplyDefine : ActionDefine
{
    public List<int> SupplyNumber { get; set; }
    public override ActionType GetActionType()
    {
        return ActionType.Supply;
    }
}

//攻击类行动
public class AttackDefine : ActionDefine
{
    public float Level { get; set; } //攻击力
    public int Damage { get; set; } //伤害
    public int AttackType { get; set; } //攻击类别
    public int Victim { get; set; } //受害者，不一定是Target
    public virtual void OnDefended(Player attacker, Player victim) { }
    public virtual void OnCountered(Player attacker, Player victim, CounterMethod counter) { }
    public virtual void OnAttacking(Player attacker, Player victim) { }
    public virtual void OnOverwhelmed(Player attacker, Player victim) { }
    public Action OnBlocking { get; set; }//攻击被防御时
    // 实现 ICloneable 接口
    public void LaserCannon()
    {
        PlayerManager.Instance.Players.TryGetValue(Target, out var targetPlayer);
        Damage = targetPlayer.status.HP.Value;
        Costs[0] = Math.Max(2, targetPlayer.status.HP.Value);
    }
    public override ActionType GetActionType()
    {
        return ActionType.Attack;
    }
}

//防御类行动
public class DefendDefine : ActionDefine
{
    public override ActionType GetActionType()
    {
        return ActionType.Defend;
    }
}

//反制类行动
public class CounterDefine : ActionDefine
{
}

//特殊行动，单独定义
public class SpecialDefine : ActionDefine
{
}