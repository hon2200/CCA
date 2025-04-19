using Common.Data;
using GameServer.Managers;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;
using static UnityEngine.GraphicsBuffer;

// AI决策上下文（新增类）
public class AIDecisionContext
{
    public int SelfHp;
    public int SelfBullet;
    public int SelfSwardCount;
    public Dictionary<int, TargetInfo> Enemies;
    public List<ItemDefine> AvailableCards;
}

// 主线程任务派发器（新增组件）
public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();

    public static void Enqueue(System.Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (_executionQueue.Count > 0)
        {
            System.Action action;
            lock (_executionQueue)
            {
                action = _executionQueue.Dequeue();
            }
            action?.Invoke();
        }
    }
}
public struct TargetInfo
{
    public int id;
    public int Hp;
    public int Bullet;
    public int SwardCount;
}
public class AIMove:MonoBehaviour
{
    public Character owner;
    private Dictionary<int, int> _cardCooldownTracker = new Dictionary<int, int>();
    private int id;
    private int Hp;
    private int Bullet;
    private int SwardCount;
    private int SwardInCD;
    public int Id => id;
    private int cardid;
    private List<int> targetid;
    private Dictionary<int, TargetInfo> targets=new Dictionary<int, TargetInfo>();

    private Thread _decisionThread;
    private bool _isRunning = true;
    private readonly object _lock = new object();
    private AIDecisionContext _currentContext;

    private void OnEnable()
    {
        Rule.OnRoundChanged += Changed;
        gameObject.AddComponent<MainThreadDispatcher>();

    }

    private void OnDisable()
    {
        Rule.OnRoundChanged -= Changed;
        _isRunning = false;
        _decisionThread?.Join();
    }
    private void StartDecisionProcess()
    {
        // 创建决策上下文快照
        var context = new AIDecisionContext
        {
            SelfHp = Hp,
            SelfBullet = Bullet,
            SelfSwardCount = SwardCount,
            Enemies = new Dictionary<int, TargetInfo>(targets),
            AvailableCards = DataManager.Instance.Items.Values.ToList()
        };

        // 启动决策线程
        _decisionThread = new Thread(() =>
        {
            var result = CalculateOptimalAction(context);

            MainThreadDispatcher.Enqueue(() =>
            {
                if (result != null)
                {
                    cardid = result.Value.card.ID;
                    targetid = result.Value.targets;
                    Battle.Instance.Add(id, cardid, targetid);
                }
            });
        });

        _decisionThread.Start();
    }

    private (ItemDefine card, List<int> targets)? CalculateOptimalAction(AIDecisionContext context)
    {
        // 第一阶段：威胁评估
        var threatLevels = context.Enemies.Values
            .OrderByDescending(e => CalculateThreatScore(e))
            .ToList();

        // 第二阶段：动态权重计算
        var weightedCards = context.AvailableCards
            .Where(CheckCostAvailability)
            .Select(c => new {
                Card = c,
                Weight = CalculateCardWeight(c, threatLevels)
            })
            .OrderByDescending(x => x.Weight)
            .ToList();
        foreach (var card in weightedCards)
        {
            if (!_isRunning) return null; // 线程中断检测
            var targets = SelectTargets(card.Card, threatLevels);
            if (targets.Count > 0)
            {
                UpdateCooldown(card.Card.ID); // 应用冷却
                return (card.Card, targets);
            }
                
        }
        return null;
    }
    private float CalculateThreatScore(TargetInfo enemy)
    {
        // 综合血量、武器数量、攻击潜力计算
        return (100 - enemy.Hp) * 0.6f +
               (enemy.Bullet + enemy.SwardCount) * 0.4f;
    }
    private float CalculateCardWeight(ItemDefine card, List<TargetInfo> threats)
    {
        float weight = 0;

        // 基础属性权重
        switch (card.ItemType)
        {
            case ItemType.近战:
            case ItemType.远程:
                weight = card.Hurt * 5 - card.Cost * 0.3f;
                if (threats.Any(t => t.Hp <= card.Hurt))
                    weight += 30; // 斩杀奖励
                break;

            case ItemType.补给:
                weight = (card.CostType == "子弹" ?
                    (10 - Bullet) * 0.8f :
                    (8 - SwardCount) * 0.6f) - Mathf.Abs(card.Cost) * 0.2f;
                break;
        }

        // 情境加成
        weight += CalculateCounterBonus(card, threats);
        return weight;
    }
    private List<int> SelectTargets(ItemDefine card, List<TargetInfo> threats)
    {
        var validTargets = threats
            .Where(t => t.id != this.id) // 关键修改：排除自身ID
            .ToList();

        return card.TargetType switch
        {
            TargetType.敌方单人 => validTargets
                .OrderByDescending(t => CalculateThreatScore(t))
                .Take(1)
                .Select(t => t.id)
                .ToList(),

            TargetType.敌方全体 => validTargets
                .Where(t => t.Hp < 30 && t.id != this.id) // 双重过滤
                .Select(t => t.id)
                .ToList(),

            _ => new List<int>()
        };
    }
    private float CalculateCounterBonus(ItemDefine card, List<TargetInfo> threats)
    {
        float bonus = 0;

        // 防御类卡牌反制预测（参考网页7的选择节点逻辑）
        if (card.ItemType == ItemType.防御 && card.DEFType != null)
        {
            var predictedActions = PredictEnemyActions(threats);
            bonus += card.DEFType.Intersect(predictedActions).Count() * 15f;
        }

        // 反制类卡牌即时响应（参考网页6的全局变量中断机制）
        if (card.ItemType == ItemType.反制)
        {
            int lastEnemyAction = Room.Instance.GetLastEnemyAction(Id);
            bonus += card.ParryType.Contains(lastEnemyAction) ? 50f : 0f;
        }

        // 群体攻击卡权重衰减（网页3的威胁响应优化）
        if (card.TargetType == TargetType.敌方全体 && threats.Count > 2)
        {
            bonus += threats.Count * 2f;
        }

        return bonus;
    }
    // 敌方行动预测（网页3的状态评估模型）
    private List<int> PredictEnemyActions(List<TargetInfo> threats)
    {
        var mainThreat = threats.OrderByDescending(t => t.Bullet + t.SwardCount).First();
        return mainThreat.Bullet > mainThreat.SwardCount ?
            new List<int> { 14, 16, 18 } :  // 预测远程攻击
            new List<int> { 8, 9, 10 };     // 预测近战连击
    }
    private bool CheckCostAvailability(ItemDefine card)
    {
        // 回合数限制检查（网页3的环境调整原则）
        if (Battle.Instance.currentRound == 1 && (card.ID == 29 || card.ID == 30))
            return false;

        // 冷却系统检查（网页8的卡牌属性系统）
        if (_cardCooldownTracker.ContainsKey(card.ID))
        {
            int cooldownRemain = _cardCooldownTracker[card.ID] - Battle.Instance.currentRound;
            if (cooldownRemain >= 0) return false;
        }

        // 原有资源检查逻辑
        if (card.ID == 17) // 激光炮特殊规则
            return Bullet >= 1;

        // 常规资源动态计算
        return card.CostType switch
        {
            "子弹" => Bullet >= Mathf.Max(1, Mathf.Abs(card.Cost)),
            "剑" => SwardCount >= Mathf.Abs(card.Cost) &&
                   SwardInCD == 0, // 增加冷却检测
            _ => true
        };
    }

    private void UpdateCooldown(int cardId)
    {
        // 记录冷却回合（网页7的卡表更新机制）
        if (cardId == 29) // "过来"卡牌
            _cardCooldownTracker[cardId] = Battle.Instance.currentRound + 1;
    }

    public void Set(int ids)
    {
        id = ids;
        Changed();
    }
    public void Changed()
    {
        lock (_lock)
        {
            targets.Clear();
            foreach (var sq in BattleManager.Instance.Characters)
            {
                if (sq.Value.ID == id)
                {
                    Hp = (int)sq.Value.HP;
                    Bullet = sq.Value.BulletCount;
                    SwardCount = sq.Value.SwordCount;
                    SwardInCD = sq.Value.SwordCD;
                }
            }
            
            foreach (var target in BattleManager.Instance.Characters.Values)
            {
                TargetInfo targetInfo = new TargetInfo();
                targetInfo.id = target.ID;
                targetInfo.Hp = (int)target.HP;
                targetInfo.Bullet = (int)target.BulletCount;
                targetInfo.SwardCount = target.SwordCount;
                targets.Add(target.ID, targetInfo);
            }
            _currentContext = new AIDecisionContext
            {
                Enemies = targets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
        StartDecisionProcess();
    }
}
