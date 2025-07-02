using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


//我担心直到这里构建起来的AILogic会导致各种内存浪费和反复读取的功能浪费...
public class AILogic : MonoBehaviour
{
    public Player player;
    public void AIMove()
    {
        var newAction = GenerateRandomAction();
        player.action.ReadinMove(newAction.ID, newAction.Target, "AI");
    }
    private ActionDefine GenerateRandomAction()
    {
        List<ActionDefine> availableActions = new();
        foreach(var action in ActionDataBase.Instance.ActionDictionary)
        {
            if (action.Value.TargetType == TargetType.Enemy)
                foreach(var target in PlayerManager.Instance.Players.Values)
                {
                    if (target.status.life.Value)
                    {
                        ActionDefine newAction = (ActionDefine)action.Value.Clone();
                        newAction.Target = target.ID_inGame;
                        if (!RuleCheck.isActionFoolish(player, newAction) &&
                            !RuleCheck.isActionLegal(player, newAction))
                        {
                            availableActions.Add(newAction);
                        }
                    }
                }
            if(action.Value.TargetType == TargetType.Self)
            {
                ActionDefine newAction = (ActionDefine)action.Value.Clone();
                newAction.Target = player.ID_inGame;
                if (!RuleCheck.isActionFoolish(player, newAction) &&
                    !RuleCheck.isActionLegal(player, newAction))
                {
                    availableActions.Add(newAction);
                }
            }
        }
        System.Random rand = new ();
        return availableActions.ElementAt(rand.Next(availableActions.Count));
    }
}
