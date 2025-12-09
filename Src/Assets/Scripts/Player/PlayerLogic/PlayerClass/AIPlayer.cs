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
    public CharacterDefine CharacterDefine { get; set; }
    public Emotion Emo { get; set; }
    public bool isFriend { get; set; }
    public Honesty Honest { get; set; }
    private Dictionary<ActionType, List<ActionDefine>> availableActionsByCategory { get; set; }
    //告诉玩家的行动类别
    public Intention IntendedType { get; set; }
    public List<string> preferedAction { get; set; }
    //创造闯关过程中的AI
    public void Initialize(int ID_inGame, AIDefine aIDefine, bool isFriend, LevelDefine Level)
    {
        var availableAction = Level.GetAllUnlockedActions()
            .Concat(aIDefine.EnabledAction)
            .Except(aIDefine.DisabledAction)
            .ToList();
        base.Initialize(ID_inGame, aIDefine.Name, PlayerType.AI, aIDefine.MaxHP, aIDefine.InitialResource,
            availableAction, aIDefine.ID, aIDefine.SkillList);
        //赋值性格
        CharacterDataBase.Instance.CharacterDictionary.TryGetValue(aIDefine.CharacterID, out var characterDefine);
        if (characterDefine == null)
            Debug.Assert(false, "Can't find Character");
        CharacterDefine = characterDefine;
        //赋值情感
        Emo = new();
        //注意：情绪变化监听
        Emo.OnValueChanged += (float oldEmo, float newEmo, string message) =>
        {
            OnEmoChange();
        };
        Emo.Set(characterDefine.IniEmotion);
        //赋值诚实
        Honest = new();
        Honest.Set(characterDefine.IniHonesty);
        preferedAction = aIDefine.PreferedAction == null ? new() : aIDefine.PreferedAction;
        this.isFriend = isFriend;
        IntendedType = new();
        OnBirth?.Invoke();
    }
    //创造英雄模式中的AI
    public void Initialize(int ID_inGame, HeroDefine heroDefine)
    {
        base.Initialize(ID_inGame, heroDefine.Name, PlayerType.AI, heroDefine.MaxHP, null, null, heroDefine.ID, heroDefine.SkillIDList);
        //赋值性格
        CharacterDataBase.Instance.CharacterDictionary.TryGetValue("Friendly", out var characterDefine);
        if (characterDefine == null)
            Debug.Assert(false, "Can't find Character");
        CharacterDefine = characterDefine;
        //赋值情感
        Emo = new();
        //注意：情绪变化监听
        Emo.OnValueChanged += (float oldEmo, float newEmo, string message) =>
        {
            OnEmoChange();
        };
        Emo.Set(characterDefine.IniEmotion);
        //赋值诚实
        Honest = new();
        Honest.Set(characterDefine.IniHonesty);
        this.isFriend = false;
        IntendedType = new();
        //警戒：情绪值对受伤应激激动
        status.HP.OnValueChanged += (int oldHP, int newHP, string message) =>
        {
            if (message == "Damage")
            {
                int damageAmount = oldHP - newHP;
                DamagedReaction(damageAmount);
            }
        };
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
    #region EmotionRelated
    private void OnEmoChange()
    {
        if (Emo.Value >= CharacterDefine.MaxEmotion)
            Emo.ChangeTo(CharacterDefine.MaxEmotion);
        else if (Emo.Value <= CharacterDefine.MinEmotion)
            Emo.ChangeTo(CharacterDefine.MinEmotion);
        foreach(var emotion in EmotionDataBase.Instance.EmotionDictionary.Values)
        {
            if (emotion.EmotionalValueLowerLimit <= Emo.Value &&
                emotion.EmotionalValueUpperLimit > Emo.Value)
                Emo.emotionType = emotion.ID;
        }
    }
    public void DamagedReaction(int damageNumber)
    {
        Emo.ChangeBy(damageNumber * CharacterDefine.EmotionChange_DamagedBased);
        Honest.ChangeBy(damageNumber * CharacterDefine.HonestyChange_DamageBased);
    }
    public void DamagingReaction(int damageNummber)
    {
        Emo.ChangeBy(damageNummber * CharacterDefine.EmotionChange_DamagingBased);
    }
    public void TurnBasedChange()
    {
        Emo.ChangeBy(CharacterDefine.EmotionChange_TurnBased);
        Honest.ChangeBy(CharacterDefine.HonestyChange_TurnBased);
    }
    #endregion

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
        EmotionDataBase.Instance.EmotionDictionary.TryGetValue(Emo.emotionType, out var emotion);
        int count = 0;
        //多重攻击
        if (newAction.actionType == ActionType.Attack)
        {
            while (UnityEngine.Random.Range(0f, 1f) < emotion.MultiAttackCheckValue)
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
            while (UnityEngine.Random.Range(0f, 1f) < emotion.MultiAttackCheckValue)
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
