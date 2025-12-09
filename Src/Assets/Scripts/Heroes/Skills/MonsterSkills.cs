using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

//Chapter 1
public class ProtectingAlly : PhasebasedSkill
{
    public ProtectingAlly() : base("Save Me!!!") { }
    protected override bool AfterResolution(Player thisPlayer)
    {
        if(thisPlayer.status.life.Value == LifeStatus.Death)
        {
            BattleManager.Instance.OnDefeated.Invoke();
            return true;
        }
        return false;
    }
}

public class ToyAssemblyLine : PhasebasedSkill
{
    public ToyAssemblyLine() : base("Toy Assembly Line") { }
    List<string> Sequence = new() { "Toy Warrior", "Toy Guardian", "Toy Minion" };
    int whichOne = 0;
    List<int> Cost = new() { 3, 3, 1 };
    protected override bool OnStartPhase(Player Factory)
    {
        if (PlayerManager.Instance.EnemyReachMaxNumber())
            return false;
        if (Factory.status.resources.Bullet.Value >= Cost[whichOne] && PlayerManager.Instance.ThereisAvailablePositions(true))
        {
            Factory.status.resources.Bullet.Use(Cost[whichOne]);
            AIDataBase.Instance.AIDictionary.TryGetValue(Sequence[whichOne], out var aIDefine);
            PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
            whichOne++;
            whichOne %= 3;
            return true;
        }
        return false;

    }
}

public class CastleGuardian :TriggerSkill
{
    public CastleGuardian() : base("Castle Guardian") { }

    protected override bool OnDeath(Player castle, out bool revive)
    {
        castle.status.HP.Heal(castle.status.MaxHP);

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
        revive = true;
        return true;
    }
}

public class FightAgain : TriggerSkill
{
    public FightAgain() : base("Fight Again") { }
    protected override bool OnDeath(Player fighter,out bool revive)
    {
        fighter.status.HP.Heal(fighter.status.MaxHP / 2);
        fighter.AvailableActions.AddRange(new List<string> { "cleave" });
        revive = true;
        return true;
    }

}

public class WaraxeDanceSkill : ActionSkill
{
    public WaraxeDanceSkill() : base("Waraxe Dance", "waraxe_dance") { }
}

//Chapter 2
public class VoidServant : PhasebasedSkill
{
    public VoidServant() : base("Void Servant") { }
    protected override bool OnStartPhase(Player thisPlayer)
    {
        if (PlayerManager.Instance.EnemyReachMaxNumber())
            return false;
        if (PlayerManager.Instance.ThereisAvailablePositions(true))
        {
            AIDataBase.Instance.AIDictionary.TryGetValue("Void Beast", out var aIDefine);
            PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
            return true;
        }
        return false;
    }
}

public class Charge: PhasebasedSkill
{
    public Charge() : base("Charge") { }
    protected override bool AfterSelectingAction(Player thisPlayer)
    {
        thisPlayer.action.Clear();
        thisPlayer.action.ReadinMove("stab", PlayerManager.Instance.HumanPlayer.ID_inGame, "AI");
        return true;
    }
}

public class ChronosHand: PhasebasedSkill
{
    public ChronosHand() : base("Chronos Hand") { }
    private Dictionary<Player, List<ActionDefine>> AddingActions;
    protected override bool OnStartPhase(Player thisPlayer)
    {
        AddingActions = new();
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false), 
                out var actions);
            if (actions == null)
                return false;
            var newActions = new List<ActionDefine>();
            foreach(var action in actions)
            {
                var newAction = (ActionDefine)action.Clone();
                newAction.Costs = new() { 0, 0, 0 };
                newActions.Add(newAction);
            }
            AddingActions.Add(player, newActions);
        }
        return true;
    }
    public override void InvokeChasePhase(Player thisPlayer)
    {
        foreach (var kvp in AddingActions)
        {
            kvp.Key.action.AddRange(kvp.Value, "Add_AI");
        }
        AddingActions.Clear();
    }


}

public class MnemosyneHand : TriggerSkill
{
    public MnemosyneHand() : base("Mnemosyne Hand") { }
    protected override bool OnDeath(Player self, out bool revive)
    {
        revive = true;
        var VoidBeasts = PlayerManager.Instance.FindSomeone("Void Beast");
        foreach(var player in VoidBeasts)
        {
            self.status.HP.Heal(4);
            Cocooned cocooned = new(3);
            cocooned.OnRevive += (self) =>
            {
                int stealSword = PlayerManager.Instance.HumanPlayer.status.resources.Sword.Value / 2;
                PlayerManager.Instance.HumanPlayer.status.resources.Sword.Lost(stealSword);
                self.status.resources.Sword.Get(stealSword);
            };
            self.status.buffs.Apply(cocooned);
            return true;
        }
        return false;
    }
}

public class MorphingRemain : PhasebasedSkill
{
    public MorphingRemain() : base("Morphing Remain") { }
    protected override bool OnStartPhase(Player thisPlayer)
    {
        int aliveAlly = 0;
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            if(player.hero.ID == "Shadow of Domination" && player.status.life.Value == LifeStatus.Alive)
                aliveAlly++;
            if (player.hero.ID == "Shadow of Deception" && player.status.life.Value == LifeStatus.Alive)
                aliveAlly++;
        }
        if(aliveAlly == 0)
        {
            bool success = CallVoidBeast();
            CallVoidBeast();
            return success;
        }
        return false;
    }
    private bool CallVoidBeast()
    {
        if (PlayerManager.Instance.EnemyReachMaxNumber())
            return false;
        if (PlayerManager.Instance.ThereisAvailablePositions(true))
        {
            AIDataBase.Instance.AIDictionary.TryGetValue("Void Beast", out var aIDefine);
            PlayerManager.Instance.CreateAI(aIDefine, false, LevelManager.Instance.GetCurrentLevel());
            return true;
        }
        return false;
    }

}

public class BiteYouToDeath : PhasebasedSkill
{
    public BiteYouToDeath() : base("Bite You to Death!") { }
    protected override bool OnChasePhase(Player thisPlayer)
    {
        var VoidBeasts = PlayerManager.Instance.FindSomeone("Void Beast");
        if (VoidBeasts.Count == 0)
            return false;
        foreach(var player in VoidBeasts)
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
        return true;
    }
}

public class DarkEmbrace : PhasebasedSkill
{
    public DarkEmbrace() : base("Dark Embrace") { }
    protected override bool OnStartPhase(Player thisPlayer)
    {
        int number = PlayerManager.Instance.HumanPlayer.status.resources.Sword.Value;
        PlayerManager.Instance.HumanPlayer.status.resources.Sword.Lost(number);
        thisPlayer.status.resources.Sword.Get(number);
        return true;
    }
}

public class EnlightenmentonHighFort1 : TriggerSkill
{
    public int Combo = 0;
    public EnlightenmentonHighFort1() : base("Enlightenment on High Fort1") { }
    protected override bool OnAttackTakeEffect(Player attacker, Player victim, AttackDefine attack)
    {
        //刀剑类攻击
        if (attack.Costs[2] > 0)
        {
            Combo++;
            return false;
        }
        return false;
    }
    protected override bool OnDamaged(Player attacker, Player victim, int damage)
    {
        Combo = 0;
        return false;
    }
}

public class EnlightenmentonHighFort2 : PhasebasedSkill
{
    public EnlightenmentonHighFort2() : base("Enlightenment on High Fort2") { }
    private int GetCombo(Player thisPlayer)
    {
        return thisPlayer.hero.GetSkill<EnlightenmentonHighFort1>()?.Combo ?? 0;
    }
    protected override bool BeforeResolution(Player thisPlayer)
    {
        bool hasAttack = false;
        int Combo = GetCombo(thisPlayer);
        if (Combo == 0)
            return false;
        //加强刀剑攻击力
        foreach(var action in thisPlayer.action)
        {
            if (action is AttackDefine attack && action.Costs[2] > 0)
            {
                hasAttack = true;
                attack.Level += Combo;
            }

        }
        //刀剑重复执行一次
        for(int i = 0; i < Combo; i++)
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
        return hasAttack;
    }
}

public class CriticalStrike : PhasebasedSkill
{
    bool Critical = false;
    public CriticalStrike() : base("Critical Strike") { }
    protected override bool BeforeResolution(Player thisPlayer)
    {
        if(Critical)
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
                foreach(var action in actionSnapShot)
                {
                    if (action is AttackDefine || action is DefendDefine || action is CounterDefine)
                        victim.action.Remove(action);
                }
            }
            if (!Critical)
                PrintEvent.Instance.log += ("剑心发动会心\n");
        }
        return false;
    }
    private int GetCombo(Player thisPlayer)
    {
        return thisPlayer.hero.GetSkill<EnlightenmentonHighFort1>()?.Combo ?? 0;
    }
    protected override bool OnStartPhase(Player thisPlayer)
    {
        if (GetCombo(thisPlayer) == 1 && !Critical) 
        {
            Critical = true;
            return true;
        }
        return false;
    }
}

public class Adjudication : PhasebasedSkill
{
    public int Last = 0;
    public Adjudication() : base("Adjudication") { }
    //这只是对行动的一个模仿，我认为人机的这种东西没必要写成一个行动
    protected override bool OnChasePhase(Player thisPlayer)
    {
        thisPlayer.action.Clear();
        Last = 2;
        return true;
    }
    public override void InvokeAfterSelectingAction(Player thisPlayer)
    {
        if (Last > 0)
        {
            foreach (var player in PlayerManager.Instance.Players.Values)
            {
                if (player is AIPlayer ai && !ai.isFriend)
                {
                    foreach (var action in ai.action)
                    {
                        action.Costs = new List<int> { Math.Max(0, action.Costs[0]-1),
                        Math.Max(0, action.Costs[1]-1),
                        Math.Max(0, action.Costs[2]-1)};
                    }
                }
            }
            Last--;
            PrintEvent.Instance.log += ($"本回合敌方所有消耗-1，还有{Last}回合\n");
        }
    }
}

public class Judgement : PhasebasedSkill
{
    private bool Active = false;
    public Judgement() : base("Judgement") { }
    //在前一回合发动时申明
    protected override bool OnChasePhase(Player thisPlayer)
    {
        thisPlayer.action.Clear();
        Active = true;
        PlayerManager.Instance.HumanPlayer.status.buffs.Apply(new Crystallized(3));
        return true;
    }
    public override void InvokeStartPhase(Player thisPlayer)
    {
        if(Active)
        {
            Active = false;
            foreach (var actionID in PlayerManager.Instance.HumanPlayer.AvailableActions)
                if (ActionUtil.IsAction<AttackDefine>(actionID))
                    PlayerManager.Instance.HumanPlayer.ForbiddenActions.Add(actionID);
        }
    }
}

public class PrismaticEssence : TriggerSkill
{
    public PrismaticEssence() : base("Prismatic Essence") { }
    protected override bool OnAttackTakeEffect(Player attacker, Player victim, AttackDefine attack)
    {
        if (ActionUtil.IsAttackLight(attack))
        {
            victim.status.buffs.Apply(new Crystallized(2));
            return true;
        }
        return false;
    }
}

public class TacticalTurtle1 : PhasebasedSkill
{
    public bool Turtling = true;
    public bool LoseAttack = false;
    public TacticalTurtle1() : base("Tactical Turtle1") { }
    public override void InvokeStartPhase(Player thisPlayer)
    {
        if(!LoseAttack)
        {
            LoseAttack = true;
            thisPlayer.status.buffs.Apply(new Strength(-2));
        }
        if (Turtling)
        {
            Turtling = false;
            foreach(var action in thisPlayer.AvailableActions)
            {
                if (ActionUtil.IsAction<AttackDefine>(action))
                    thisPlayer.ForbiddenActions.Add(action);
            }
        }

    }
}

public class TacticalTurtle2 : TriggerSkill
{
    public TacticalTurtle2() : base("Tactical Turtle2") { }
    protected override bool OnAttacked(Player attacker, Player victim, AttackDefine attack)
    {
        attack.Damage -= 1;
        return true;
    }
    protected override bool OnDamaged(Player attacker, Player victim, int damage)
    {
        var turtle = victim.hero.GetSkill<TacticalTurtle1>();
        turtle.Turtling = true;
        return true;
    }
}

public class BlazingArmor : TriggerSkill
{
    public BlazingArmor() : base("Blazing Armor") { }
    protected override bool OnAttacked(Player attacker, Player victim, AttackDefine attack)
    {
        attacker.status.buffs.Apply(new Burning(2));
        if (ActionUtil.IsAttackLight(attack))
            attack.Damage = 0;
        return true;
    }
}

public class BeatDown : TriggerSkill
{
    public bool beating = false;
    public BeatDown() : base("Beat Down") { }
    protected override bool OnAttackOverwhelmed(Player attacker, Player enemy, AttackDefine attack)
    {
        if (!beating)
            return false;
        //创建并添加攻击特效
        EffectManager.Instance.PlayTrailEffect(false, "Bullet", attacker.gameObject, enemy.gameObject);
        var counters = attack.WatchoutforCounter(enemy);
        var defends = attack.WatchoutforDefend(enemy);
        //对应防御反击判断
        if (counters.Count > 0)
        {
            foreach (var counter in counters)
            {
                counter.Item1.HowtoCounter(counter.Item2, attacker, enemy, attack);
                attack.OnCountered(attacker, enemy, counter.Item2);
            }
        }
        else if (defends.Count > 0)
        {
            foreach (var defend in defends)
            {
                defend.HowtoDefend(attack, enemy);
                attack.OnDefended(attacker, enemy);
            }
        }
        //总算是命中了！
        else
        {
            attack.HowtoAttack(attacker, enemy);
            attack.OnAttacking(attacker, enemy);
            foreach (var skill in attacker.hero.skills)
            {
                if (skill is TriggerSkill triggerSkill)
                    triggerSkill.InvokeOnAttackTakeEffect(attacker, enemy, attack);
            }
        }

        return true;
    }
    protected override bool OnDamaging(Player attacker, Player victim, int damage)
    {
        if (!beating)
            return false;
        victim.status.buffs.Apply(new Strength(-1));
        return true;
    }
}

public class ReadytoBeatDown : PhasebasedSkill
{
    public ReadytoBeatDown(): base("Ready to Beat Down") { }
    protected override bool OnStartPhase(Player thisPlayer)
    {
        var beatDown = thisPlayer.hero.GetSkill<BeatDown>();
        if (beatDown != null)
        {
            beatDown.beating = true;
            return beatDown.beating;
        }
        return false;
    }
}

public class Photophobia : TriggerSkill
{
    public Photophobia() : base("Photophobia") { }
    protected override bool OnAttacked(Player attacker, Player victim, AttackDefine attack)
    {
        if (ActionUtil.IsAttackLight(attack))
        {
            attack.Damage += 2;
            return true;
        }
        return false;
    }

}