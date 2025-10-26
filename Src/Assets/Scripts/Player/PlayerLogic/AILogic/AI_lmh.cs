using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;


//AI 的逻辑拓展类
//用于处理AI行动的方式
public class AI_lmh
{
    public Player thisPlayer { get; set; }
    //所有人的可用行动字典，分类别存放
    private Dictionary<(int,ActionType),List<ActionDefine>> AvailableActions { get; set; }
    public AI_lmh(Player thisPlayer)
    {
        this.thisPlayer = thisPlayer;
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
    public Dictionary<ActionDefine,double> CalculateAttackActionProb(int target)
    {
        List<AttackDefine> AvailableAttack = thisPlayer.CheckAction<AttackDefine>(target);
        PlayerManager.Instance.Players.TryGetValue(target, out var enemy);
        Dictionary<AttackDefine, double> ScoreDic = new();
        foreach(var originAttack in AvailableAttack)
        {
            var newAttack = (AttackDefine)originAttack.Clone();
            newAttack.Target = target;
            double base_score = newAttack.Damage + newAttack.Level * 0.5;
            double cost_term = (newAttack.Costs.Sum()) *
                (thisPlayer.status.resources.Bullet.Value + thisPlayer.status.resources.Sword.Value) / 2;
            double enemy_health_term = (enemy.status.HP.Value < 3 ? 1 : 0) * 0.5 * newAttack.Damage;
            double defense_panelty_term = CalculateDefensePenalty(newAttack);
            double score = base_score + cost_term + enemy_health_term - defense_panelty_term;
            double action_score = Math.Max(score, 0.01);
            ScoreDic.Add(newAttack, action_score);
        }
        return ScoreToProb(ScoreDic.ConvertToParentDictionary<ActionDefine, AttackDefine, double>());
    }
    //计算防御反弹行动概率分布
    public Dictionary<ActionDefine,double> CalculateDefendActionProb(List<Player> Enemies)
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
    public Dictionary<ActionDefine, double> CalculateCounterActionProb(List<Player> Enemies)
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
}

