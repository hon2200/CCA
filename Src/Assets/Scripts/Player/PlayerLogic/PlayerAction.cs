using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction
{
    //这回合做出的行动：打出的卡牌
    public List<ActionBase> ActionList { get; set; }
    //行动历史//使用整个变量存储//可以访问到历史行动经过一系列buff后的最终结算参数
    public Dictionary<(int, bool), List<ActionBase>> LongHistory { get; set; }
    public PlayerAction()
    {
        ActionList = new();
        LongHistory = new();
    }

    //读入历史行动
    public void ReadinHistory(bool isProcessed)
    {
        List<ActionBase> newHistory_l = new List<ActionBase>();
        foreach (ActionBase action in ActionList)
        {
            //拷贝一份行动进history
            newHistory_l.Add((ActionBase)action.Clone());
        }
        LongHistory.Add((BattleManager.Instance.Turn.Value, isProcessed), ActionList);
    }
    //通过动态类型构造，实现简洁漂亮的代码
    public void ReadinMove(string ID, int Target)
    {
        if (ActionDataBase.Instance.ActionDictionary.TryGetValue(ID, out var value))
        {
            // 1. 获取 value 的实际类型（如 AttackDefine）
            Type actionDefineType = value.GetType();

            // 2. 构造泛型 BattleAction<T> 类型（如 BattleAction<AttackDefine>）
            Type actionGenericType = typeof(BattleAction<>).MakeGenericType(actionDefineType);

            // 3. 动态创建实例
            //注意，这里使用value的深拷贝
            object actionInstance = Activator.CreateInstance(
                actionGenericType,
                (ActionDefine)value.Clone(),
                Target
            );

            // 4. 添加到 ActionList（ActionList 是 List<ActionBase>）
            ActionList.Add((ActionBase)actionInstance);
        }
        else
        {
            Debug.Assert(false, "Wrong ID");
        }
    }
    public void ClearMove()
    {
        ActionList.Clear();
    }
}
