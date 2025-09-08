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
    public AIDefine AIDefine { get; set; }
    public CharacterDefine CharacterDefine { get; set; }
    public Emotion Emo { get; set; }
    public bool isFriend { get; set; }
    public Honesty Honest { get; set; }
    private Dictionary<ActionType, List<ActionDefine>> availableActionsByCategory { get; set; }
    //告诉玩家的行动类别
    public Intention IntendedType { get; set; }
    public void Initialize(int ID_inGame, AIDefine aIDefine, bool isFriend)
    {
        HeroDataBase.Instance.HeroDictionary.TryGetValue("Blank", out var heroDefine);
        base.Initialize(ID_inGame, PlayerType.AI, heroDefine);
        //重新修改最大生命值
        this.status = new(aIDefine.MaxHP, aIDefine.InitialResource);
        //处理可用行动
        this.AvailableActions = aIDefine.AvailableActionID;
        //赋值
        AIDefine = aIDefine;
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
        this.isFriend = isFriend;
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
        Emo.ChangeBy(damageNumber * CharacterDefine.EmotionChange_DamageBased);
        Honest.ChangeBy(damageNumber * CharacterDefine.HonestyChange_DamageBased);
    }
    public void TurnBasedChange()
    {
        Emo.ChangeBy(CharacterDefine.EmotionChange_TurnBased);
        Honest.ChangeBy(CharacterDefine.HonestyChange_TurnBased);
    }
    #endregion

    #region MoveLogic
    // 添加常量定义
    private static readonly Dictionary<int, ActionType> IndexToActionType = new Dictionary<int, ActionType>
    {
        { 0, ActionType.Supply },
        { 1, ActionType.Attack },
        { 2, ActionType.Defend },
        { 3, ActionType.Counter },
        { 4, ActionType.Special }
    };

    private static readonly Dictionary<ActionType, int> ActionTypeToIndex = new Dictionary<ActionType, int>
    {
        { ActionType.Supply, 0 },
        { ActionType.Attack, 1 },
        { ActionType.Defend, 2 },
        { ActionType.Counter, 3 },
        { ActionType.Special, 4 }
    };

    private static readonly Dictionary<ActionType, string> ActionTypeNames = new Dictionary<ActionType, string>
    {
        { ActionType.Supply, "补给" },
        { ActionType.Attack, "攻击" },
        { ActionType.Defend, "防御" },
        { ActionType.Counter, "反制" },
        { ActionType.Special, "特效" }
    };

    public void EmotionalAIMove()
    {
        var newAction = GenerateAccordingToTendency();
        action.ReadinMove(newAction.ID, newAction.Target, "AI");
        IntendedType.Set(DecideToTellAction(newAction.GetActionType()));
        isReady.ReadyUp();
    }

    //要不要告诉玩家呢？
    public ActionType DecideToTellAction(ActionType realActionType)
    {
        float honestValue = Honest.Value;
        // 验证输入
        if (honestValue < 0f || honestValue > 1f)
        {
            Debug.LogError($"诚实值 {honestValue} 超出范围 [0, 1]，已限制到有效范围");
            honestValue = Mathf.Clamp01(honestValue);
        }

        if (availableActionsByCategory == null || availableActionsByCategory.Count == 0)
        {
            Debug.LogError("行动类别列表为空！");
            return realActionType; //  fallback
        }

        // 根据诚实值决定是否说真话
        float randomValue = UnityEngine.Random.Range(0f, 1f);

        if (randomValue <= honestValue)
        {
            // 说真话
            Debug.Log($"诚实值 {honestValue} 触发，告诉真实行动: {realActionType}");
            return realActionType;
        }
        else
        {
            // 说假话 - 从其他类别中随机选择一个
            ActionType fakeActionType = GetRandomFakeActionType(realActionType);
            Debug.Log($"诚实值 {honestValue} 未触发，告诉虚假行动: {fakeActionType} (真实: {realActionType})");
            return fakeActionType;
        }
    }

    //按倾向生成行动
    private ActionDefine GenerateAccordingToTendency()
    {
        List<int> Tendency = Emo.GetTendency();
        StringBuilder AIThinkingProcess = new StringBuilder();

        // 检查输入有效性
        if (Tendency == null || Tendency.Count != 5)
        {
            Debug.LogError("Tendency列表必须包含5个权重值");
            return null;
        }

        AIThinkingProcess.Append("\n\n" + "现在是第" + BattleManager.Instance.Turn.Value + "回合");
        AIThinkingProcess.Append("\n" + ID_inGame + "玩家我的行为倾向现在是" + string.Join(" ,", Tendency));
        AIThinkingProcess.Append("\n" + "我的诚实值是" + Honest.Value + "待会你就知道我会不会骗你了...");
        AIThinkingProcess.Append("\n" + "我的情绪值是" + Emo.Value +
            "\n" + "我现在很" + Emo.emotionType +
            "\n" + "我开始思考......");

        // 第一步：为每个行动类别生成所有可用行动
        availableActionsByCategory = new Dictionary<ActionType, List<ActionDefine>>();

        foreach (var actionType in IndexToActionType.Values)
        {
            List<ActionDefine> categoryActions = GenerateAvailableActionsForCategory(actionType);
            availableActionsByCategory[actionType] = categoryActions;

            AIThinkingProcess.Append($"\n{ActionTypeNames[actionType]} 类别有 {categoryActions.Count} 个可用行动分别是\n");
            foreach (var action in categoryActions)
            {
                AIThinkingProcess.Append(action.Name + " ,");
            }
        }

        // 第二步：根据权重选择类别
        ActionType selectedActionType = SelectCategoryByWeight(Tendency, availableActionsByCategory);

        if (selectedActionType == ActionType.Origin) // 假设有None作为默认值
        {
            AIThinkingProcess.Append("\n所有类别都没有可用行动！");
            MyLog.WriteToFile("Assets/Log/InGame/AIThinking.txt", AIThinkingProcess, false);
            return null;
        }

        // 第三步：从选定的类别中随机选择一个行动
        List<ActionDefine> selectedCategoryActions = availableActionsByCategory[selectedActionType];
        System.Random rand = new System.Random();
        ActionDefine selectedAction = selectedCategoryActions[rand.Next(selectedCategoryActions.Count)];

        AIThinkingProcess.Append($"\n最终选择: {ActionTypeNames[selectedActionType]} 类别的 {selectedAction.ID}");
        MyLog.WriteToFile("Assets/Log/InGame/AIThinking.txt", AIThinkingProcess, false);

        return selectedAction;
    }

    // 按类生成可用行动，以ActionType为参数，并且返回一个List
    private List<ActionDefine> GenerateAvailableActionsForCategory(ActionType actionType)
    {
        List<ActionDefine> availableActions = new List<ActionDefine>();

        switch (actionType)
        {
            case ActionType.Supply:
                return GenerateActionsForType<SupplyDefine>();
            case ActionType.Attack:
                return GenerateActionsForType<AttackDefine>();
            case ActionType.Defend:
                return GenerateActionsForType<DefendDefine>();
            case ActionType.Counter:
                return GenerateActionsForType<CounterDefine>();
            case ActionType.Special:
                return GenerateActionsForType<SpecialDefine>();
        }

        return availableActions;
    }

    //按类生成可用行动，以Type为参数，并且将结果添加到List
    private List<ActionDefine> GenerateActionsForType<Type>() where Type : ActionDefine
    {
        List<ActionDefine> availableActions = new();
        var actionTypeDic = ActionDataBase.Instance.GetActionType<Type>();

        if (actionTypeDic == null || actionTypeDic.Count == 0)
        {
            Debug.LogWarning($"没有找到 {typeof(Type).Name} 类型的行动");
        }

        foreach (var action in actionTypeDic)
        {
            if (action.Value.TargetType == TargetType.Enemy)
            {
                foreach (var target in PlayerManager.Instance.Players.Values)
                {
                    ActionDefine newAction = (ActionDefine)action.Value.Clone();
                    newAction.Target = target.ID_inGame;
                    if (IsActionValid(newAction))
                    {
                        availableActions.Add(newAction);
                    }
                }
            }

            if (action.Value.TargetType == TargetType.Self)
            {
                ActionDefine newAction = (ActionDefine)action.Value.Clone();
                newAction.Target = ID_inGame;
                if (IsActionValid(newAction))
                {
                    availableActions.Add(newAction);
                }
            }
        }
        return availableActions;
    }

    // 检查行动是否有效
    private bool IsActionValid(ActionDefine action)
    {
        return !RuleCheck.isActionFoolish(this, action) &&
               RuleCheck.isActionLegal(this, action) &&
               RuleCheck.isActionAvailable(this, action);
    }

    // 修改权重选择方法，直接返回 ActionType
    private ActionType SelectCategoryByWeight(List<int> tendency, Dictionary<ActionType, List<ActionDefine>> availableActionsByCategory)
    {
        // 创建带权重和可用行动数量的类别列表
        List<(ActionType actionType, int weight, int availableCount)> weightedCategories = new List<(ActionType, int, int)>();

        foreach (var actionType in IndexToActionType.Values)
        {
            int index = ActionTypeToIndex[actionType];
            int availableCount = availableActionsByCategory[actionType].Count;

            if (availableCount > 0) // 只考虑有可用行动的类别
            {
                weightedCategories.Add((actionType, tendency[index], availableCount));
            }
        }

        if (weightedCategories.Count == 0)
        {
            return ActionType.Origin; // 没有可用行动
        }

        // 计算总权重（考虑可用行动数量）
        float totalWeight = 0f;
        foreach (var category in weightedCategories)
        {
            // 权重乘以可用行动数量的平方根，避免数量影响过大
            float adjustedWeight = category.weight * Mathf.Sqrt(category.availableCount);
            totalWeight += adjustedWeight;
        }

        // 随机选择
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var category in weightedCategories)
        {
            float adjustedWeight = category.weight * Mathf.Sqrt(category.availableCount);
            currentWeight += adjustedWeight;

            if (randomValue <= currentWeight)
            {
                return category.actionType;
            }
        }

        return weightedCategories[0].actionType; // fallback
    }

    // 修改 GetRandomFakeActionType 方法
    private ActionType GetRandomFakeActionType(ActionType realActionType)
    {
        // 直接使用 availableActionsByCategory 的 Keys
        var allActionTypes = availableActionsByCategory
            .Where(kvp => kvp.Value.Count > 0)
            .Select(kvp => kvp.Key)
            .ToList();

        // 过滤掉真实类别
        var availableFakeTypes = allActionTypes.Where(type => type != realActionType).ToList();

        if (availableFakeTypes.Count == 0)
        {
            Debug.LogWarning("没有其他行动类别可用于生成假信息，返回真实类别");
            return realActionType;
        }

        // 随机选择一个假类别
        int randomIndex = UnityEngine.Random.Range(0, availableFakeTypes.Count);
        return availableFakeTypes[randomIndex];
    }

    // 修改 GetCategoryName 方法（如果需要）
    private string GetCategoryName(ActionType actionType)
    {
        return ActionTypeNames.TryGetValue(actionType, out string name) ? name : "未知";
    }
    #endregion
}
