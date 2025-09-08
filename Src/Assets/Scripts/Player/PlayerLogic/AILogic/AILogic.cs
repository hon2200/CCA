using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//我担心直到这里构建起来的AILogic会导致各种内存浪费和反复读取的功能浪费...
//这个类停用
public class AILogic
{
    public AIDefine aiDefine;
    private AIPlayer aiPlayer;
    //老东西了
    // 改进的GenerateSpecificAction方法，添加更多调试信息
    private ActionDefine GenerateSpecificAction<Type>() where Type : ActionDefine
    {
        List<ActionDefine> availableActions = new();
        var actionTypeDic = ActionDataBase.Instance.GetActionType<Type>();

        if (actionTypeDic == null || actionTypeDic.Count == 0)
        {
            Debug.LogWarning($"没有找到 {typeof(Type).Name} 类型的行动");
            return null;
        }

        foreach (var action in actionTypeDic)
        {
            if (action.Value.TargetType == TargetType.Enemy)
            {
                foreach (var target in PlayerManager.Instance.Players.Values)
                {
                    if (target.status.life.Value == LifeStatus.Alive)
                    {
                        ActionDefine newAction = (ActionDefine)action.Value.Clone();
                        newAction.Target = target.ID_inGame;
                        CheckAction(availableActions, newAction);
                    }
                    else
                    {
                        Debug.Log($"跳过目标 {target.ID_inGame} - 已死亡");
                    }
                }

            }

            if (action.Value.TargetType == TargetType.Self)
            {
                ActionDefine newAction = (ActionDefine)action.Value.Clone();
                newAction.Target = aiPlayer.ID_inGame;
                CheckAction(availableActions, newAction);

            }
        }

        if (availableActions.Count == 0)
        {
            return null;
        }

        System.Random rand = new();
        return availableActions[rand.Next(availableActions.Count)];
    }

    private void CheckAction(List<ActionDefine> availableActions,ActionDefine newAction)
    {
        StringBuilder checkLog = new StringBuilder();
        checkLog.Append($"\nPlayer" + aiPlayer.ID_inGame + "检查自身行动 " + newAction.ID + ":\n ");

        bool isFoolish = RuleCheck.isActionFoolish(aiPlayer, newAction);
        bool isLegal = RuleCheck.isActionLegal(aiPlayer, newAction);
        bool isAvailable = RuleCheck.isActionAvailable(aiPlayer, newAction);

        if (!isFoolish && isLegal && isAvailable)
        {
            availableActions.Add(newAction);
            checkLog.Append("✓ 所有检查通过");
        }
        else
        {
            if (isFoolish)
            {
                checkLog.Append("✗ 愚蠢行动 ");
            }
            if (!isLegal)
            {
                checkLog.Append("✗ 非法行动 ");
            }
            if (!isAvailable)
            {
                checkLog.Append("✗ 不可用行动 ");
            }
        }
        MyLog.WriteToFile("Assets/Log/InGame/ActionCheck.txt", checkLog, false);
    }
}

