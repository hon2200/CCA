#region Old Skills
/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

//注意：调用技能的时候不要判断是否Available，因为你有些技能可能有多阶段效果，每次都判断一下是不合理的
//另外，有很多技能还有局内判断是否可用的逻辑。
//写技能因此要检查几点：1.Availability.
//Chapter 1
public class ProtectingAlly : Skill, IResolutionHandler
{
    public ProtectingAlly() : base("Save Me!!!") { }
    protected override bool IsAvailable(Player thisPlayer)
    {
        return base.IsAvailable(thisPlayer) && thisPlayer.status.life.Value == LifeStatus.Death;
    }
    public void AfterResolution(Player thisPlayer)
    {
        CheckAndEvoke(thisPlayer);
    }
    protected override void Envoke(Player thisPlayer)
    {
        BattleManager.Instance.OnDefeated.Invoke();
    }
}

public class ToyAssemblyLine : Skill, IPhaseEnterHandler
{
    public ToyAssemblyLine() : base("Toy Assembly Line") { }
    List<string> Sequence = new() { "Toy Warrior", "Toy Guardian", "Toy Minion" };
    int whichOne = 0;
    List<int> Cost = new() { 3, 3, 1 };
    protected override bool IsAvailable(Player thisPlayer)
    {
        return base.IsAvailable(thisPlayer) && thisPlayer.status.resources.Bullet.Value >= Cost[whichOne] && PlayerManager.Instance.ThereisAvailablePositions(true);
    }
    protected override void Envoke(Player thisPlayer)
    {
        thisPlayer.status.resources.Bullet.Use(Cost[whichOne]);
        AIDataBase.Instance.AIDictionary.TryGetValue(Sequence[whichOne], out var aIDefine);
        PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
        whichOne++;
        whichOne %= 3;
    }
    public void OnPhase(Phase phase, Player Factory)
    {
        if (phase is StartPhase)
            CheckAndEvoke(Factory);
    }
}

public class CastleGuardian : Skill, IDeathHandler
{
    public CastleGuardian() : base("Castle Guardian") { }
    protected override void Envoke(Player castle)
    {
        castle.status.HP.Heal(castle.status.MaxHP);

        //删掉原来的技能
        var toRemove = new List<Skill>();

        foreach (Skill skill in castle.hero.skills)
        {
            if (skill.ID == "Toy Assembly Line")
                toRemove.Add(skill);
        }

        foreach (Skill skill in toRemove)
            castle.hero.skills.Remove(skill);

        castle.AvailableActions.AddRange(new List<string> { "shoot", "double_shoot" });

        if (castle is AIPlayer aiCastle)
        {
            CharacterDataBase.Instance.CharacterDictionary.TryGetValue("Bellicose", out var bellicose);
            aiCastle.CharacterDefine = bellicose;
        }
    }

    public bool OnDeath(Player castle)
    {
        return CheckAndEvoke(castle);
    }
}

public class FightAgain : Skill, IDeathHandler
{
    public FightAgain() : base("Fight Again") { }
    protected override void Envoke(Player fighter)
    {
        fighter.status.HP.Heal(fighter.status.MaxHP / 2);
        fighter.AvailableActions.AddRange(new List<string> { "cleave" });
    }
    public bool OnDeath(Player fighter)
    {
        return CheckAndEvoke(fighter);
    }

}

public class WaraxeDanceSkill : ActionSkill
{
    public WaraxeDanceSkill() : base("Waraxe Dance", "waraxe_dance") { }
}

//Chapter 2
public class VoidServant : Skill, IPhaseEnterHandler
{
    public VoidServant() : base("Void Servant") { }
    protected override bool IsAvailable(Player thisPlayer)
    {
        return base.IsAvailable(thisPlayer) && PlayerManager.Instance.ThereisAvailablePositions(true);
    }
    protected override void Envoke(Player thisPlayer)
    {
        AIDataBase.Instance.AIDictionary.TryGetValue("Void Beast", out var aIDefine);
        PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
        {
            CheckAndEvoke(thisPlayer);
        }
    }
}

public class Charge : Skill, IPhaseExitHandler
{
    public Charge() : base("Charge") { }
    protected override void Envoke(Player thisPlayer)
    {
        thisPlayer.action.Clear();
        thisPlayer.action.ReadinMove("stab", PlayerManager.Instance.HumanPlayer.ID_inGame, "AI");
    }
    public void ExitingPhase(Phase phase, Player thisPlayer)
    {
        if (phase is ActionPhase)
            CheckAndEvoke(thisPlayer);
    }
}

public class ChronosHand : Skill, IPhaseEnterHandler
{
    public ChronosHand() : base("Chronos Hand") { }
    private Dictionary<Player, List<ActionDefine>> AddingActions;
    protected override bool IsAvailable(Player thisPlayer)
    {
        return base.IsAvailable(thisPlayer) && BattleManager.Instance.Turn.Value >= 2;
    }
    protected override void Envoke(Player thisPlayer)
    {
        AddingActions = new();
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            if (actions == null)
                continue;
            var newActions = new List<ActionDefine>();
            foreach (var action in actions)
            {
                var newAction = (ActionDefine)action.Clone();
                newAction.Costs = new() { 0, 0, 0 };
                newActions.Add(newAction);
            }
            AddingActions.Add(player, newActions);
        }
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
        {
            CheckAndEvoke(thisPlayer);
        }
        else if (phase is ChasePhase)
        {
            if (AddingActions != null)
            {
                foreach (var kvp in AddingActions)
                {
                    kvp.Key.action.AddRange(kvp.Value, "Add_AI");
                }
                AddingActions.Clear();
            }
        }
    }
}

public class MnemosyneHand : Skill, IDeathHandler
{
    public MnemosyneHand() : base("Mnemosyne Hand") { }
    protected override bool IsAvailable(Player thisPlayer)
    {
        var VoidBeasts = PlayerManager.Instance.FindSomeone("Void Beast");
        return base.IsAvailable(thisPlayer) && VoidBeasts.Count > 0;
    }
    protected override void Envoke(Player thisPlayer)
    {
        var VoidBeasts = PlayerManager.Instance.FindSomeone("Void Beast");
        foreach (var player in VoidBeasts)
        {
            thisPlayer.status.HP.Heal(4);
            Cocooned cocooned = new(3);
            cocooned.OnRevive += (self) =>
            {
                int stealSword = PlayerManager.Instance.HumanPlayer.status.resources.Sword.Value / 2;
                PlayerManager.Instance.HumanPlayer.status.resources.Sword.Lost(stealSword);
                self.status.resources.Sword.Get(self, stealSword);
            };
            thisPlayer.status.buffs.Apply(cocooned);
        }
    }
    public bool OnDeath(Player self)
    {
        return CheckAndEvoke(self);
    }
}

public class MorphingRemain : Skill, IPhaseEnterHandler
{
    public MorphingRemain() : base("Morphing Remain") { }
    protected override bool IsAvailable(Player thisPlayer)
    {
        int aliveAlly = 0;
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            if (player.hero.ID == "Shadow of Domination" && player.status.life.Value == LifeStatus.Alive)
                aliveAlly++;
            if (player.hero.ID == "Shadow of Deception" && player.status.life.Value == LifeStatus.Alive)
                aliveAlly++;
        }
        return base.IsAvailable(thisPlayer) && aliveAlly == 0 &&
            PlayerManager.Instance.ThereisAvailablePositions(true);
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
        {
            CheckAndEvoke(thisPlayer);
        }
    }
    protected override void Envoke(Player thisPlayer)
    {
        CallVoidBeast();
        if (PlayerManager.Instance.ThereisAvailablePositions(true))
            CallVoidBeast();
    }
    private void CallVoidBeast()
    {
        AIDataBase.Instance.AIDictionary.TryGetValue("Void Beast", out var aIDefine);
        PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
    }

}

public class BiteYouToDeath : Skill, IPhaseEnterHandler
{
    public BiteYouToDeath() : base("Bite You to Death!") { }
    protected override bool IsAvailable(Player thisPlayer)
    {
        var VoidBeasts = PlayerManager.Instance.FindSomeone("Void Beast");
        return base.IsAvailable(thisPlayer) && VoidBeasts.Count > 0;
    }
    protected override void Envoke(Player thisPlayer)
    {
        var VoidBeasts = PlayerManager.Instance.FindSomeone("Void Beast");
        foreach (var player in VoidBeasts)
        {
            var human = PlayerManager.Instance.HumanPlayer;
            var beastStab = player.action.ReadinMove("stab", human.ID_inGame, "AI");
            beastStab.Costs = new() { 0, 0, 0 };
            if (beastStab is AttackDefine beastStabing)
            {
                beastStabing.OnAttackingAction += (player, human) =>
                {
                    human.status.buffs.Apply(new Bleeding(2));
                };
            }
            else
                Debug.Assert(false, "Stab is not Attack");
        }
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is ChasePhase)
        {
            CheckAndEvoke(thisPlayer);
        }
    }
}

public class DarkEmbrace : Skill, IPhaseEnterHandler
{
    public DarkEmbrace() : base("Dark Embrace") { }
    protected override void Envoke(Player thisPlayer)
    {
        int number = PlayerManager.Instance.HumanPlayer.status.resources.Sword.Value;
        PlayerManager.Instance.HumanPlayer.status.resources.Sword.Lost(number);
        thisPlayer.status.resources.Sword.Get(thisPlayer, number);
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
            CheckAndEvoke(thisPlayer);
    }
}

public class EnlightenmentonHighFort : Skill, ICombatHandler, IDamagedHandler, IPhaseExitHandler
{
    public int Combo = 0;
    public EnlightenmentonHighFort() : base("Enlightenment on High Fort") { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        if (combatEvent.Type == CombatEventType.AttackTakeEffect)
        {
            //刀剑类攻击
            if (combatEvent.Attack.Costs[2] > 0)
            {
                Combo++;
            }
        }
    }
    public void OnDamaged(Player attacker, Player victim, int damage)
    {
        Combo = 0;
    }
    protected override bool IsAvailable(Player thisPlayer)
    {
        return base.IsAvailable(thisPlayer) && Combo > 0;
    }
    protected override void Envoke(Player thisPlayer)
    {
        //加强刀剑攻击力
        foreach (var action in thisPlayer.action)
        {
            if (action is AttackDefine attack && action.Costs[2] > 0)
            {
                attack.Level += Combo;
            }
        }
        //刀剑重复执行一次
        for (int i = 0; i < Combo; i++)
        {
            var attackSnapShot = thisPlayer.SelectActionType<AttackDefine>().ToList();
            foreach (var attack in attackSnapShot)
            {
                if (attack.Costs[2] > 0)
                {
                    var newAttack = (AttackDefine)attack.Clone();
                    newAttack.Costs = new() { 0, 0, 0 };
                    thisPlayer.action.Add(newAttack, "AI_Add");
                }
            }
        }
    }
    public void ExitingPhase(Phase phase, Player thisPlayer)
    {
        if (phase is ActionPhase)
        {
            CheckAndEvoke(thisPlayer);
        }
    }

}

public class CriticalStrike : Skill, IPhaseExitHandler, IPhaseEnterHandler
{
    bool Critical = false;
    public CriticalStrike() : base("Critical Strike") { }
    protected override bool IsAvailable(Player thisPlayer)
    {
        var attackSnapShot = thisPlayer.SelectActionType<AttackDefine>().ToList();
        //只在不是会心状态以及连击第二次时发动
        return base.IsAvailable(thisPlayer) && GetCombo(thisPlayer) == 1 && !Critical;
    }
    public void ExitingPhase(Phase phase, Player thisPlayer)
    {
        if (phase is ActionPhase)
        {
            if (Critical)
            {
                var attackSnapShot = thisPlayer.SelectActionType<AttackDefine>().ToList();
                foreach (var attack in attackSnapShot)
                {
                    PlayerManager.Instance.Players.TryGetValue(attack.Target, out var victim);
                    var actionSnapShot = victim.action.ToList();
                    Critical = false;
                    bool beProvoke = false;
                    foreach (var action in actionSnapShot)
                    {
                        if (action.ID == "provoke")
                        {
                            thisPlayer.action.Remove(attack, "Remove_AI");
                            beProvoke = true;
                        }
                    }
                    if (beProvoke)
                        continue;
                    foreach (var action in actionSnapShot)
                    {
                        if (action is AttackDefine || action is DefendDefine || action is CounterDefine)
                            victim.action.Remove(action);
                    }
                }
                if (!Critical)
                    PrintEvent.Instance.log += ("剑心发动会心\n");
            }
        }
    }
    private int GetCombo(Player thisPlayer)
    {
        return thisPlayer.hero.GetSkill<EnlightenmentonHighFort>()?.Combo ?? 0;
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
            CheckAndEvoke(thisPlayer);
    }
    protected override void Envoke(Player thisPlayer)
    {
        Critical = true;
    }
}

public class OdetoMajesty : Skill, IActionReplacer, IResolutionHandler
{
    public int Last = 0;
    public OdetoMajesty() : base("Ode to Majesty") { }
    //这只是对行动的一个模仿，我认为人机的这种东西没必要写成一个行动
    protected override void Envoke(Player thisPlayer)
    {
        thisPlayer.action.Clear();
        Last = 2; base.Envoke(thisPlayer);
    }
    public void ReplaceAction(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer))
        {
            CheckAndEvoke(thisPlayer);
        }
    }
    public void AfterResolution(Player thisPlayer)
    {
        if (Last > 0)
        {
            foreach (var player in PlayerManager.Instance.Players.Values)
            {
                if (player is AIPlayer ai && !ai.isFriend)
                {
                    foreach (var action in ai.action)
                    {
                        if (action.Costs[1] > 0 || action.Costs[2] > 0)
                            player.status.HP.Heal(1);
                    }
                }
            }
            Last--;
            PrintEvent.Instance.log += ($"本回合敌方消耗资源回复体力，还有{Last}回合\n");
        }
    }
}

public class Judgement : Skill, IActionReplacer, IPhaseEnterHandler
{
    private bool Active = false;
    public Judgement() : base("Judgement") { }
    protected override void Envoke(Player thisPlayer)
    {
        thisPlayer.action.Clear();
        Active = true;
        PlayerManager.Instance.HumanPlayer.status.buffs.Apply(new Crystallized(3));
    }
    //在前一回合发动时申明
    public void ReplaceAction(Player thisPlayer)
    {
        if (IsAvailable(thisPlayer))
        {
            Envoke(thisPlayer);
        }
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase && Active)
        {
            Active = false;
            foreach (var actionID in PlayerManager.Instance.HumanPlayer.AvailableActions)
                if (ActionUtil.IsAction<AttackDefine>(actionID))
                    PlayerManager.Instance.HumanPlayer.ForbiddenActions.Add(actionID);
        }
    }
}

public class PrismaticEssence : Skill, ICombatHandler
{
    public PrismaticEssence() : base("Prismatic Essence") { }
    public void OnCombatEvent(CombatEvent evt)
    {
        if (ActionUtil.IsAttackLight(evt.Attack))
        {
            evt.Victim.status.buffs.Apply(new Crystallized(2));
        }
    }
}

public class TacticalTurtle : Skill, ICombatHandler, IPhaseEnterHandler, IDamagedHandler
{
    public int Turtling = 0;
    public bool LoseAttack = false;
    public TacticalTurtle() : base("Tactical Turtle") { }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
        {
            if (!LoseAttack)
            {
                LoseAttack = true;
                thisPlayer.status.buffs.Apply(new Strength(-2));
            }
            //上一回合进入龟缩
            if (Turtling == 2)
            {
                //这一回合不能使用所有行动
                Turtling--;
                foreach (var action in thisPlayer.AvailableActions)
                {
                    if (ActionUtil.IsAction<AttackDefine>(action))
                        thisPlayer.ForbiddenActions.Add(action);
                }
            }
            //结束状态
            if (Turtling == 1)
                Turtling--;
        }
    }
    public void OnCombatEvent(CombatEvent evt)
    {
        if (evt.Type == CombatEventType.Attacked)
        {
            evt.Attack.Damage -= 1;
            //进入龟缩状态后
            if (Turtling == 1)
            {
                evt.Attack.Damage = 0;
            }
        }
    }
    public void OnDamaged(Player attacker, Player victim, int damage)
    {
        Turtling = 2;
    }
}

public class BlazingArmor : Skill, ICombatHandler
{
    public BlazingArmor() : base("Blazing Armor") { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        combatEvent.Attacker.status.buffs.Apply(new Burning(2));
        if (ActionUtil.IsAttackLight(combatEvent.Attack))
            combatEvent.Attack.Damage = 0;
    }
}

public class BeatDown : Skill, ICombatHandler, IPhaseEnterHandler
{
    public bool beating = false;
    public BeatDown() : base("Beat Down") { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        if (beating)
        {
            if (combatEvent.Type == CombatEventType.AttackOverwhelmed)
            {
                combatEvent.Attack.HowtoAttack(combatEvent.Attacker, combatEvent.Victim);
            }
            else if (combatEvent.Type == CombatEventType.Attacking)
            {
                combatEvent.Victim.status.buffs.Apply(new Strength(-1));
            }
        }
    }
    public void OnPhase(Phase phase, Player thisPlayer)
    {
        if (phase is StartPhase)
        {
            CheckAndEvoke(thisPlayer);
        }
    }
    protected override void Envoke(Player thisPlayer)
    {
        beating = true;
    }
}

public class Photophobia : Skill, ICombatHandler
{
    public Photophobia() : base("Photophobia") { }

    public void OnCombatEvent(CombatEvent combatEvent)
    {
        if (combatEvent.Type == CombatEventType.Attacked)
            if (ActionUtil.IsAttackLight(combatEvent.Attack))
            {
                combatEvent.Attack.Damage += 2;
                CheckAndEvoke(combatEvent.Victim);
            }
    }

}*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoltenCast : EnemySkill, IPhaseEnterHandler
{
    public MoltenCast() : base("Molten Cast") { }
    protected override void Envoke()
    {
        if (BattleManager.Instance.Turn.Value == 9)
        {
            var players = PlayerManager.Instance.FindSomeone("Bronze Imp");
            foreach (var imp in players)
            {
                imp.status.HP.Damage(999, Owner, imp, null);
                Owner.status.buffs.Add(new DamagingOperator(
                    new BuffOperator.Step(BuffOperator.OpType.Multiply, 2), Owner));
                Owner.status.buffs.Add(new AttackingLevelOperator(
                    new BuffOperator.Step(BuffOperator.OpType.Add, 0.5f), Owner));
            }
        }
    } 
    public void OnPhase(Phase phase)
    {
        if(phase is StartPhase)
        {
            CheckAndEvoke();
        }
    }
}

public class Sow : SummoningSkill, IPhaseEnterHandler
{
    private int count = 1;
    public Sow() : base("Sow") { CDProgress = 1; }
    protected override void Envoke()
    {
        Summon("Imp");
    }
    public void OnPhase(Phase phase)
    {
        CheckAndEvoke();
    }
}