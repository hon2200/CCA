using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 非泛型基类
public abstract class ActionBase : ICloneable
{
    public ActionDefine ActionInfo { get; protected set; }
    public int Target { get; set; }
    //暂时这个值只在攻击行动里面使用，判断行动是否生效。未来所以其他行动都可能用到
    public bool isEffective { get; set; }
    public abstract object Clone();
}

// 泛型实现
public class BattleAction<T> : ActionBase where T : ActionDefine
{
    //注意，当BattleAction拉取Actioninfo的相关信息的时候，使用的是深拷贝
    //这样的操作实际上让基类的ActionInfo（事实上）成为字段，而这里的ActionInfo成为属性。
    //我们这里使用基类Protected Set，事实上感觉未必有必要？因为未来可能一些英雄的基类属性会变，比如cost什么的，虽然可以通过其子类改，但是有时候可能不太方便，这个Protected Set留着待定
    //这里我们的子类的set直接去修改base的Action info，get直接获得base的Actioninfo，也事实上造成两个类的ActionInfo完全一致。
    public new T ActionInfo
    {
        get => (T)base.ActionInfo;
        set => base.ActionInfo = value;
    }

    public BattleAction(T actionInfo, int target)
    {
        ActionInfo = actionInfo;
        Target = target;
        isEffective = false;
    }
    public BattleAction(T actionInfo, int target,bool isEffective_)
    {
        ActionInfo = actionInfo;
        Target = target;
        isEffective = isEffective_;
    }
    public override object Clone()
    {
        // 创建新的 BattleAction<T>，并深拷贝所有字段
        return new BattleAction<T>(
            (T)this.ActionInfo.Clone(), // 再次深拷贝 ActionInfo
            this.Target,
            this.isEffective
        );
    }
}
