using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;


//AI 的逻辑拓展类
//用于处理AI行动的方式
public class AI_lmh
{
    public AIPlayer thisPlayer { get; set; }
    private List<string> preferedAction { get; set; }
    //所有人的可用行动字典，分类别存放
    private Dictionary<(int,ActionType),List<ActionDefine>> AvailableActions { get; set; }
    public AI_lmh(AIPlayer thisPlayer)
    {
        this.thisPlayer = thisPlayer;
        preferedAction = thisPlayer.preferedAction;
        AvailableActions = new();
        InitializeAvailableActions();
    }
    //初始化敌人的可用行动
    public void InitializeAvailableActions()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            foreach (ActionType actionType in Enum.GetValues(typeof(ActionType)))
            {
                if(actionType!= ActionType.Origin)
                {
                    if(actionType == ActionType.Attack)
                    {
                        //假想对方以自己为敌
                        AvailableActions.Add((player.ID_inGame, ActionType.Attack),
                            player.CheckAction(ActionType.Attack,thisPlayer.ID_inGame));
                    }
                    else
                    {
                        AvailableActions.Add((player.ID_inGame, actionType),
                            player.CheckAction(actionType));
                    }
                }
            }
        }
    }
    public ActionDefine SmartSelectAction(ActionType actionType, List<ActionDefine> availableActionList)
    {
        if (availableActionList.Count == 0 || availableActionList == null)
            Debug.Assert(false, "No Available");
        System.Random rand = new System.Random();
        switch (actionType)
        {
            case ActionType.Supply:
                var supply = availableActionList[rand.Next(availableActionList.Count)];
                //没有选到就再选一次//目前只先处理supply
                if (!preferedAction.Contains(supply.ID) && preferedAction.Count != 0)
                    supply = availableActionList[rand.Next(availableActionList.Count)];
                supply.Target = thisPlayer.ID_inGame;
                return supply;
            case ActionType.Attack:
                var players = PlayerManager.Instance.GetAlivePlayers();
                Player victim = players[rand.Next(players.Count)];
                while (thisPlayer.CheckAction(ActionType.Attack,victim.ID_inGame).Count == 0)
                {
                    players.Remove(victim);
                    victim = players[rand.Next(players.Count)];
                }
                return RandomSelection(CalculateAttackActionProb(victim.ID_inGame));
            case ActionType.Defend:
                return RandomSelection(CalculateDefendActionProb(thisPlayer.GetEnemy()));
            case ActionType.Counter:
                return RandomSelection(CalculateCounterActionProb(thisPlayer.GetEnemy()));
            case ActionType.Special:
                return availableActionList[rand.Next(availableActionList.Count)];
            default:
                Debug.Assert(false, "No Type Available");
                return null;
        }
    }
    private T RandomSelection<T>(Dictionary<T, double> ProbDic)
    {
        // 计算总权重
        float totalWeight = 0;
        foreach (var category in ProbDic)
        {
            totalWeight += (float)category.Value;
        }

        // 随机选择
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0f;

        foreach (var category in ProbDic)
        {
            float adjustedWeight = (float)category.Value;
            currentWeight += adjustedWeight;

            if (randomValue <= currentWeight)
            {
                return category.Key;
            }
        }

        Debug.Assert(false, "No Selection Available");
        return default(T); // fallback
    }
    //计算我的攻击行动可能因为敌方防御受到的惩罚
    private double CalculateDefensePenalty(AttackDefine attack)
    {
        double penalty = 0;
        AvailableActions.TryGetValue((attack.Target, ActionType.Defend), out var defends);
        AvailableActions.TryGetValue((attack.Target, ActionType.Counter), out var counters);
        foreach(var defend in defends)
        {
            ActionDataBase.Instance.VersusTable.TryGetValue((attack.ID, defend.ID), out var counterMethod);
            if (counterMethod == CounterMethod.Block)
                penalty += 0.5;  
        }
        foreach (var counter in counters)
        {
            ActionDataBase.Instance.VersusTable.TryGetValue((attack.ID, counter.ID), out var counterMethod);
            if (counterMethod == CounterMethod.Rebounce)
            {
                penalty += 0.5;
                penalty += 0.5 * attack.Damage;
            }
            else if(counterMethod == CounterMethod.Disarm)
            {
                penalty += 0.75;
            }

        }
        return penalty;
    }
    //计算攻击行动概率分布
    private Dictionary<ActionDefine,double> CalculateAttackActionProb(int target)
    {
        List<AttackDefine> AvailableAttack = thisPlayer.CheckAction<AttackDefine>(target);
        PlayerManager.Instance.Players.TryGetValue(target, out var enemy);
        Dictionary<AttackDefine, double> ScoreDic = new();
        foreach(var originAttack in AvailableAttack)
        {
            var newAttack = (AttackDefine)originAttack.Clone();
            newAttack.Target = target;
            double base_score = newAttack.Damage + newAttack.Level * 0.5;
            double cost_term = (newAttack.Costs.Sum()) * Math.Min
                (thisPlayer.status.resources.Bullet.Value + thisPlayer.status.resources.Sword.Value,
                newAttack.Costs.Sum() * 2) / 10;
            double enemy_health_term = (enemy.status.HP.Value < 3 ? 1 : 0) * 0.5 * newAttack.Damage;
            double defense_panelty_term = CalculateDefensePenalty(newAttack);
            double score = base_score + cost_term + enemy_health_term - defense_panelty_term;
            double action_score = Math.Max(score, 0.01);
            ScoreDic.Add(newAttack, action_score);
        }
        return ScoreToProb(ScoreDic.ConvertToParentDictionary<ActionDefine, AttackDefine, double>());
    }
    //计算防御反弹行动概率分布
    private Dictionary<ActionDefine,double> CalculateDefendActionProb(List<Player> Enemies)
    {
        
        List<DefendDefine> defends = thisPlayer.CheckAction<DefendDefine>();
        Dictionary<DefendDefine, double> ScoreDic = new();
        foreach (var originDefend in defends)
        {
            var defend = (DefendDefine)originDefend.Clone();
            var newAttack = (DefendDefine)originDefend.Clone();
            double base_score = 1;
            double effectiveness = 0;
            foreach(var enemy in Enemies)
            {
                foreach(var attack in enemy.CheckAction<AttackDefine>(thisPlayer.ID_inGame))
                {
                    ActionDataBase.Instance.VersusTable.TryGetValue((attack.ID, defend.ID),out var counterMethod);
                    if (counterMethod == CounterMethod.Block)
                        effectiveness += 1;
                    else if (counterMethod == CounterMethod.Rebounce)
                        effectiveness += (1 + attack.Damage);
                    else if (counterMethod == CounterMethod.Disarm)
                        effectiveness += 1.5;
                }
            }
            ScoreDic.Add(defend,Math.Max(base_score * effectiveness, 0.01));
        }
        return ScoreToProb(ScoreDic.ConvertToParentDictionary<ActionDefine, DefendDefine, double>());
    }
    private Dictionary<ActionDefine, double> CalculateCounterActionProb(List<Player> Enemies)
    {
        List<CounterDefine> defends = thisPlayer.CheckAction<CounterDefine>();
        Dictionary<CounterDefine, double> ScoreDic = new();
        foreach (var originDefend in defends)
        {
            var defend = (CounterDefine)originDefend.Clone();
            var newAttack = (CounterDefine)originDefend.Clone();
            double base_score = 1;
            double effectiveness = 0;
            foreach (var enemy in Enemies)
            {
                foreach (var attack in enemy.CheckAction<AttackDefine>(thisPlayer.ID_inGame))
                {
                    ActionDataBase.Instance.VersusTable.TryGetValue((attack.ID, defend.ID), out var counterMethod);
                    if (counterMethod == CounterMethod.Block)
                        effectiveness += 1;
                    else if (counterMethod == CounterMethod.Rebounce)
                        effectiveness += (1 + attack.Damage);
                    else if (counterMethod == CounterMethod.Disarm)
                        effectiveness += 1.5;
                }
            }
            ScoreDic.Add(defend, Math.Max(base_score * effectiveness, 0.01));
        }
        return ScoreToProb(ScoreDic.ConvertToParentDictionary<ActionDefine, CounterDefine, double>());
    }
    private Dictionary<ActionDefine,double> ScoreToProb(Dictionary<ActionDefine,double> ScoreDic)
    {
        List<double> probs = new();
        List<double> ScoreList = new();
        foreach(var score in ScoreDic.Values)
        {
            ScoreList.Add(score);
        }
        foreach (var score in ScoreList)
        {
            double exp_scores = Math.Exp(score - ScoreList.Sum());
            double prob = exp_scores / ScoreList.Sum();
            probs.Add(prob);
        }
        var result = ScoreDic.Keys.Zip(probs, (key, prob) => new { key, prob })
                  .ToDictionary(x => x.key, x => x.prob);
        return result;
    }
    //I think it's ok to randomize the supply and special actions
    //Now: first generate all available actions, to see if a catagory is empty
    //Select all none-empty catagories, gives a probabilty, and select one specific. If attack is selected, its target will be randomaized
    public ActionDefine GenerateAction()
    {
        List<int> Tendency = thisPlayer.Emo.GetTendency();
        StringBuilder AIThinkingProcess = new StringBuilder();

        // 检查输入有效性
        if (Tendency == null || Tendency.Count != 5)
        {
            Debug.LogError("Tendency列表必须包含5个权重值");
            return null;
        }
        AIThinkingProcess.Append("\n\n" + "现在是第" + BattleManager.Instance.Turn.Value + "回合");
        AIThinkingProcess.Append("\n" + thisPlayer.ID_inGame + "玩家我的行为倾向现在是" + string.Join(" ,", Tendency));
        AIThinkingProcess.Append("\n" + "我已经做了以下行动：" + "\n");
        foreach(var action in thisPlayer.action)
        {
            AIThinkingProcess.Append(action.ID + "目标 " + action.Target + "  ");
        }
        AIThinkingProcess.Append("\n" + "我的情绪值是" + thisPlayer.Emo.Value +
            "\n" + "我现在很" + thisPlayer.Emo.emotionType +
            "\n" + "我开始思考......");
        // 第一步：为每个行动类别生成所有可用行动
        var availableActionsByCategory = new Dictionary<ActionType, List<ActionDefine>>();
        foreach (ActionType actionType in Enum.GetValues(typeof(ActionType)))
        {
            if (actionType == ActionType.Origin)
                continue;
            List<ActionDefine> categoryActions = thisPlayer.CheckAllAction(actionType);
            availableActionsByCategory[actionType] = categoryActions;

            AIThinkingProcess.Append($"\n{actionType} 类别有 {categoryActions.Count} 个可用行动分别是\n");
            foreach (var action in categoryActions)
            {
                AIThinkingProcess.Append(action.Name + " 目标" + action.Target + " ,");
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

        // 第三步：从选定的类别中通过权重的方式选择一个行动
        var mySelection = SmartSelectAction(selectedActionType, availableActionsByCategory[selectedActionType]);
        
        AIThinkingProcess.Append($"\n最终选择: {selectedActionType} 类别的 {mySelection.ID}");

        // 最后，检查一下是不是应该挑衅
        System.Random rand = new System.Random();
        var provokers = CheckProvoke();
        if (provokers.Count > 0)
        {
            var attack = SpecificAttack(provokers[rand.Next(provokers.Count)]);
            if (attack != null)
            {
                AIThinkingProcess.Append($"\n不行,我被挑衅了——我必须反击！");
                mySelection = attack;
                Debug.Log("给他的颜色看看！");
            }
        }

        MyLog.WriteToFile("Assets/Log/InGame/AIThinking.txt", AIThinkingProcess, false);
        return mySelection;
    }
    private ActionType SelectCategoryByWeight(List<int> tendency, Dictionary<ActionType, List<ActionDefine>> availableActionsByCategory)
    {
        // 创建带权重和可用行动数量的类别列表
        List<(ActionType actionType, int weight, int availableCount)> weightedCategories = new List<(ActionType, int, int)>();

        foreach (ActionType actionType in Enum.GetValues(typeof(ActionType)))
        {
            if(actionType == ActionType.Origin)
                continue;
            //Original Takes the zero point, so the real index should be 1 less.
            int index = (int)actionType - 1;
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

        // 计算总权重
        float totalWeight = 0f;
        foreach (var category in weightedCategories)
        {
            totalWeight += category.weight;
        }

        // 随机选择
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var category in weightedCategories)
        {
            currentWeight += category.weight;

            if (randomValue <= currentWeight)
            {
                return category.actionType;
            }
        }

        return weightedCategories[0].actionType; // fallback
    }

    private List<int> CheckProvoke()
    {
        List<int> provokers = new();
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false), out var actions);
            if(actions != null)
            {
                foreach(var action in actions)
                {
                    if (action.ID == "provoke" && action.Target == thisPlayer.ID_inGame)
                        provokers.Add(player.ID_inGame); 
                }
            }
        }
        return provokers;
    }

    private ActionDefine SpecificAttack(int target)
    {
        List<AttackDefine> specificAttacks = new();
        PlayerManager.Instance.Players.TryGetValue(target, out var player);
        if (player == null || player.status.life.Value == LifeStatus.Death)
            return null;
        else if (thisPlayer.CheckAction(ActionType.Attack, target).Count == 0)
            return null;
        else
            return RandomSelection(CalculateAttackActionProb(target));
    }
}

