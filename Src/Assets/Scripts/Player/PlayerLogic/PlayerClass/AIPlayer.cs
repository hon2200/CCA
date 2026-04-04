using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//目前英雄模式的AI没有转入这些逻辑，这里的AI专门是那些有情感的家伙
//未来可以把Emotion写成Attribute，把OnEmotionChange写成委托
public class AIPlayer : Player
{
    public bool isFriend { get; set; }
    private Dictionary<ActionType, List<ActionDefine>> availableActionsByCategory { get; set; }
    //告诉玩家的行动类别
    public List<string> preferedAction { get; set; }
    //创造英雄模式中的AI
    public void Initialize(int ID_inGame, HeroDefine heroDefine, TutorialDefine tutorialDefine = null)
    {
        List<string> availableAction = null;
        if(tutorialDefine != null)
        {
            availableAction = tutorialDefine.GetAllUnlockedActions().ToList();
        }
        preferedAction = new();
        base.Initialize(ID_inGame, heroDefine.Name, PlayerType.AI, heroDefine.MaxHP, 
            null, availableAction, heroDefine.ID, heroDefine.SkillIDList);
        OnBirth?.Invoke();
    }
    public List<Player> GetEnemy()
    {
        List<Player> enemy = new List<Player>();
        if(isFriend)
        {
            foreach(var player in PlayerManager.Instance.Players.Values)
            {
                if(player is AIPlayer ai)
                {
                    if (!ai.isFriend)
                        enemy.Add(ai);
                }
            }
        }
        else
        {
            foreach (var player in PlayerManager.Instance.Players.Values)
            {
                if (player is AIPlayer ai)
                {
                    if (ai.isFriend)
                        enemy.Add(ai);
                }
                else
                    enemy.Add(player);
            }
        }
        return enemy;
    }

    #region MoveLogic

    public void EmotionalAIMove()
    {
        AI_lmh ai = new(this);
        var newAction = ai.GenerateAction();
        if (newAction == null)
        {
            Debug.Log($"{Name}Don't Have Available Actions");
            return;
        }

        action.ReadinMoveAndConsume(newAction.ID, newAction.Target, "AI", this);
        int count = 0;
        //多重攻击
        if (newAction.actionType == ActionType.Attack)
        {
            while (UnityEngine.Random.Range(0f, 1f) < 0.5)
            {
                if (this.CheckAllAction<AttackDefine>().Count > 0)
                {
                    var secondAction = ai.SmartSelectAction(ActionType.Attack, this.CheckAllAction(ActionType.Attack));
                    action.ReadinMoveAndConsume(secondAction.ID, secondAction.Target, "AI", this);
                }
                else
                {
                    break;
                }
                count++;
                if (count > 100)
                    break;
            }
        }
        //多重挑衅
        if (newAction.ID == "provoke")
        {
            while (UnityEngine.Random.Range(0f, 1f) < 0.5)
            {
                if (this.CheckAllAction<SpecialDefine>().Count > 0)
                {
                    var secondAction = ai.SmartSelectAction(ActionType.Special, this.CheckAllAction(ActionType.Special));
                    action.ReadinMove(secondAction.ID, secondAction.Target, "AI");
                }
                else
                {
                    break;
                }
                if (count > 100)
                    break;
            }
        }
        //IntendedType.Set(DecideToTellAction(newAction.GetActionType()));
        isReady.ReadyUp();
    }

    #endregion
}
