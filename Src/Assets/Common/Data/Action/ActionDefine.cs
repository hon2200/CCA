using System;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;

//行动类别
[Serializable]
public enum ActionType
{
    Supply = 1,
    BulletAttack = 2, 
    SwordAttack = 3,
    Defend = 4,
    Counter = 5,
    Special = 6,
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
    public TargetType TargetType { get; set; } // 目标类型//未来可能在读取行动上面有用
    // 实现 ICloneable 接口（深拷贝）
    public virtual object Clone()
    {
        return new ActionDefine
        {
            ID = this.ID,                // int 是值类型，直接复制
            Name = this.Name,            // string 是不可变的，直接赋值即可
            Description = this.Description,
            Costs = new List<int>(this.Costs), // 创建新 List，复制所有元素
            CD = this.CD,
            TargetType = this.TargetType,
        };
    }
}

//补给类行动
public class SupplyDefine : ActionDefine
{
    public List<int> SupplyNumber { get; set; }
    public bool isCopy { get; set; }//是否是“过来”产生的复制体，等价于：是否响应过来
    public override object Clone()
    {
        SupplyDefine copy = TypeConverter.ShallowConvertToChild<SupplyDefine,ActionDefine>
            ((ActionDefine)base.Clone());
        copy.SupplyNumber = new List<int>(this.SupplyNumber); // 创建新 List，复制所有元素
        return copy;
    }
}

//攻击类行动
public class AttackDefine : ActionDefine
{
    public float Level { get; set; } //攻击力
    public int Damage { get; set; } //伤害
    public int AttackType { get; set; } //攻击类别
    public bool isCopy { get; set; } //是否是“过来”产生的复制体，等价于：是否响应过来
    // 实现 ICloneable 接口
    public override object Clone()
    {
        // 先调用基类的 Clone() 复制基类属性
        AttackDefine copy = TypeConverter.ShallowConvertToChild<AttackDefine, ActionDefine>
            ((ActionDefine)base.Clone());

        // 复制子类新增的属性
        copy.Level = this.Level;
        copy.Damage = this.Damage;
        copy.AttackType = this.AttackType;
        copy.isCopy = this.isCopy;
        return copy;
    }
    //假设Attack生效之后调用这个函数
    public void HowtoAttack(Player attacker, Player victim, BattleAction<AttackDefine> attack)
    {
        switch(attack.ActionInfo.ID)
        {
            //溅射伤害，暂时没有通过buff去定义它，之后想到好的架构再去加，先暂时写在这里
            case "nuclear_bomb":
                victim.status.HP.Damage(attack.ActionInfo.Damage);
                foreach(var player in PlayerManager.Instance.Players)
                {
                    if (player.Key != victim.ID_inGame)
                    {
                        player.Value.status.HP.Damage(1);
                    }
                }
                break;
            default:
                victim.status.HP.Damage(attack.ActionInfo.Damage);
                break;
        }
        attack.isEffective = true;
    }
}

//防御类行动
public class DefendDefine : ActionDefine
{
    public override object Clone()
    {
        DefendDefine copy = TypeConverter.ShallowConvertToChild<DefendDefine, ActionDefine>
            ((ActionDefine)base.Clone());
        return copy;
    }
    //目前防御没有任何额外效果
    public void HowtoDefend(BattleAction<AttackDefine> attack)
    {
        attack.isEffective = false;
    }
}

//反制类行动
public class CounterDefine : ActionDefine
{
    public void HowtoCounter(CounterMethod counterType, Player attacker, Player victim, BattleAction<AttackDefine> attack)
    {
        switch(counterType)
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
                attack.ActionInfo.HowtoAttack(attacker, attacker, attack);
                break;
            default:
                throw new Exception("Wrong Counter Type");
        }
    }
    public override object Clone()
    {
        CounterDefine copy = TypeConverter.ShallowConvertToChild<CounterDefine, ActionDefine>
            ((ActionDefine)base.Clone());
        return copy;
    }
}

//特殊行动，单独定义
public class SpecialDefine : ActionDefine
{
    private bool RespondingProvoke(Player provoker, Player victim)
    {
        var attacks = victim.SelectActionType<AttackDefine>();
        foreach(var attack in attacks)
        {
            if (attack.Target == provoker.ID_inGame)
                return true;
        }
        return false;
    }
    public void OnProvoke(Player provoker, Player victim)
    {
        if (!RespondingProvoke(provoker, victim))
            victim.status.HP.Drain(1);
    }
    public override object Clone()
    {
        SpecialDefine copy = TypeConverter.ShallowConvertToChild<SpecialDefine, ActionDefine>
            ((ActionDefine)base.Clone());
        return copy;
    }
}