using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


//我担心直到这里构建起来的AILogic会导致各种内存浪费和反复读取的功能浪费...
//我担心直到这里构建起来的AILogic会导致各种内存浪费和反复读取的功能浪费...
public class AILogic
{
    public AIDefine aiDefine;
    private Player player;
    public AILogic(Player player)
    {
        this.player = player;
    }
    public void AIMove()
    {
        var newAction = GenerateSpecificAction<ActionDefine>();
        player.action.ReadinMove(newAction.ID, newAction.Target, "AI");
        player.isReady.ReadyUp();
    }
    private ActionDefine GenerateSpecificAction<Type>() where Type : ActionDefine
    {
        List<ActionDefine> availableActions = new();
        var actionTypeDic = ActionDataBase.Instance.GetActionType<Type>();
        foreach (var action in actionTypeDic)
        {
            if (action.Value.TargetType == TargetType.Enemy)
                foreach (var target in PlayerManager.Instance.Players.Values)
                {
                    if (target.status.life.Value)
                    {
                        ActionDefine newAction = (ActionDefine)action.Value.Clone();
                        newAction.Target = target.ID_inGame;
                        if (!RuleCheck.isActionFoolish(player, newAction) &&
                            RuleCheck.isActionLegal(player, newAction))
                        {
                            availableActions.Add(newAction);
                        }
                    }
                }
            if (action.Value.TargetType == TargetType.Self)
            {
                ActionDefine newAction = (ActionDefine)action.Value.Clone();
                newAction.Target = player.ID_inGame;
                if (!RuleCheck.isActionFoolish(player, newAction) &&
                    RuleCheck.isActionLegal(player, newAction))
                {
                    availableActions.Add(newAction);
                }
            }
        }
        if (availableActions.Count == 0)
        {
            Debug.Assert(false, "No Available Action!");
            return null;
        }
        System.Random rand = new();
        return availableActions.ElementAt(rand.Next(availableActions.Count));
    }
    //这个类依赖于咱们的行动类别《补给，攻击，防御，反弹，特效》
    private ActionDefine GenerateAccordingToTendecy(List<int> Tendency)
    {
        //
        switch (1)
        {
            case 1:
                return GenerateSpecificAction<SupplyDefine>();
            case 2:
                return GenerateSpecificAction<AttackDefine>();
            case 3:
                return GenerateSpecificAction<DefendDefine>();
            case 4:
                return GenerateSpecificAction<AttackDefine>();
            case 5:
                return GenerateSpecificAction<SpecialDefine>();
            default:
                return null;
        }
    }
}

