using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//PlayerAction信息管理：

/*
 * 首先：玩家的行动现在是一个Observable可观测量，也就意味着：任何对它的改变都会被检测到并会反馈一个message，
 * 以下说明对message的命名——
 * 对行动的改变目前只有三种情况可能发生：1.玩家读入，2.电脑读入，3.战斗结算中生成更改。4.战斗结束后（一般是清空行动）
 * 三种情况分别命名为Player、AI、InGame、End
 * 2.命名message的规范是Add_Player，Add为原本List的操作名，用下划线连接，两边都大写
 * 3.要处理的就是这几种情况排列组合产生的委托
 * 4.用这样的string管理可能不如enum严谨，但是enum确实也过于麻烦了，而且string可以通过提取前面几个字或者后面几个字的方式提取信息。
 * 
 */
public class PlayerAction : ObservableList<ActionBase>
{
    //这回合做出的行动：打出的卡牌
    //行动历史//使用整个变量存储//可以访问到历史行动经过一系列buff后的最终结算参数
    // key: (turnNumber, isProcessed), value: 该回合的行动快照
    public Dictionary<(int, bool), List<ActionBase>> LongHistory { get; } = new();

    // 读取当前行动到历史记录
    public void ReadinHistory(bool isProcessed)
    {
        // 创建深拷贝列表
        var historySnapshot = new List<ActionBase>(this.Count);
        foreach (var action in this) // 直接遍历当前列表
        {
            historySnapshot.Add((ActionBase)action.Clone());
        }

        // 以 (当前回合, 是否已处理) 为键存储
        LongHistory.Add((BattleManager.Instance.Turn.Value, isProcessed), historySnapshot);
    }
    public PlayerAction()
    {
        LongHistory = new();
    }
    //通过动态类型构造，实现简洁漂亮的代码
    public void ReadinMove(string ID, int Target, string Case)
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

            // 4. 添加到 Value（Value 是 List<ActionBase>）
            Add((ActionBase)actionInstance, "Add_" + Case);
        }
        else
        {
            Debug.Assert(false, "Wrong ID");
        }
    }

    public void DeleteMove(ActionBase actionBase, string Case)
    {
        Remove(actionBase, "Remove_" + Case);
    }

    public void DeleteMoveAt(int order, string Case)
    {
        RemoveAt(order, "Remove_" + Case);
    }

    public void ClearMove(string Case)
    {
        Clear("Clear_" + Case);
    }

}
