// Template implementations for all skills in EnemySkill.json.
// Base: EnemySkill (SkillDefine). Summoning skills use SummoningSkill : EnemySkill.
// Fill in Envoke() and interface method logic as needed.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Phase-based (turn/round)

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
                Owner.status.buffs.Apply(new DamagingOperator(
                    new BuffOperator.Step(BuffOperator.OpType.Multiply, 2), Owner));
                Owner.status.buffs.Apply(new AttackingLevelOperator(
                    new BuffOperator.Step(BuffOperator.OpType.Add, 0.5f), Owner));
            }
        }
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
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

public class LushGrowth : EnemySkill, IPhaseExitHandler, IDamagedHandler
{
    public LushGrowth() : base("Lush Growth") { }
    protected override void Envoke() 
    {
        Owner.status.HP.Heal(5);
    }
    public void ExitingPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
    public void OnDamaged(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = damage;
        CDProgress = CD;
    }
}

public class RapidGrowth : EnemySkill, IPhaseExitHandler
{
    public RapidGrowth() : base("Rapid Growth") { }
    protected override void Envoke() 
    {
        Owner.status.buffs.Apply(new DamagingOperator
            (new BuffOperator.Step(BuffOperator.OpType.Add, 1), Owner));
    }
    public void ExitingPhase(Phase phase)
    {
        if (phase is EndPhase)
            CheckAndEvoke();
    }
}

//注：如果未来要写一个强制敌人攻击的逻辑，这个可用
public class Tsunami : EnemySkill, IActionModifier
{
    public Tsunami() : base("Tsunami") { }
    protected override void Envoke()
    {
        var waters = PlayerManager.Instance.FindSomeone("Intangible Water");
        foreach (var water in waters)
        {
            if (water.status.life.Value != LifeStatus.Alive)
                continue;
            water.status.buffs.Apply(new DamagingOperator(
                new BuffOperator.Step(BuffOperator.OpType.Multiply, 2), water));
            // All waters do attack action if available
            var alive = PlayerManager.Instance.GetAlivePlayers();
            foreach (var target in alive)
            {
                if (target.ID_inGame == water.ID_inGame)
                    continue;
                if (water.CheckAction(ActionType.Attack, target.ID_inGame).Count == 0)
                    continue;
                var attacks = water.CheckAction<AttackDefine>(target.ID_inGame);
                if (attacks.Count == 0)
                    continue;
                water.action.ClearMove("Tsunami");
                var attack = attacks[UnityEngine.Random.Range(0, attacks.Count)];
                water.action.ReadinMoveAndConsume(attack.ID, target.ID_inGame, "Tsunami", water);
                break;
            }
        }
    }
    public void ModifyAction(Player player)
    {
        CheckAndEvoke();
    }
}

public class ScorchedOblivion : EnemySkill, ISupplyHandler, IOnKillHandler
{
    private bool _hasSuppliedOnce;

    public ScorchedOblivion() : base("Scorched Oblivion") { }
    protected override void Envoke() { }
    public void OnSupplied(Player supplier)
    {
        if (supplier != Owner) return;
        if (_hasSuppliedOnce) return;
        _hasSuppliedOnce = true;
        Owner.status.buffs.Apply(new BurnMark(10, Owner));
    }
    public void OnKill(Player killer, Player victim)
    {
        if (killer != Owner) return;
        Owner.status.buffs.Apply(new BurnMark(3, Owner));
    }
}

public class CityBurn : EnemySkill, IPhaseExitHandler
{
    public CityBurn() : base("City Burn") { }
    protected override void Envoke() { }
    public void ExitingPhase(Phase phase)
    {
        // TODO: From round 3 each round — characters who didn't consume resource take 1 from owner
    }
}

public class Conqueror : EnemySkill, IPhaseEnterHandler
{
    public Conqueror() : base("Conqueror") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
        // TODO: Odd rounds — attack damage*4, atk+0.5
    }
}

public class Invader : EnemySkill, IPhaseEnterHandler
{
    public Invader() : base("Invader") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
        // TODO: Even rounds — on damage steal half target resources (floor)
    }
}

public class DeathIncarnate : EnemySkill, IPhaseEnterHandler
{
    public DeathIncarnate() : base("Death Incarnate") { }
    protected override void Envoke()
    {
        // TODO: From round 4 every 4 rounds enter 生命收割 3 turns (no damage, damage*4); if no kill then next turn stun
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class Ritual : EnemySkill, IPhaseEnterHandler
{
    public Ritual() : base("Ritual") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
        // TODO: Every 2 rounds damage +1 (stackable)
    }
}

public class Collapse : EnemySkill, IPhaseExitHandler
{
    public Collapse() : base("Collapse") { }
    protected override void Envoke() { }
    public void ExitingPhase(Phase phase)
    {
        // TODO: When owner has most HP on field, lose 2 HP per turn
    }
}

public class HellGather : SummoningSkill, IPhaseEnterHandler
{
    public HellGather() : base("Hell Gather") { }
    protected override void Envoke()
    {
        // TODO: Every 5 rounds summon 3 Fallen Angels; max 7 on field
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class HeavenlyPunishment : EnemySkill, IPhaseEnterHandler
{
    public HeavenlyPunishment() : base("Heavenly Punishment") { }
    protected override void Envoke()
    {
        // TODO: Every 20 rounds deal 20 damage to all enemy units
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class Redemption : EnemySkill, IPhaseEnterHandler, IDeathHandler
{
    public Redemption() : base("Redemption") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
        // TODO: Round 10 — owner dies, all alive allies full heal + cleanse
    }
    public bool OnDeath(Player thisPlayer)
    {
        return false;
    }
}

public class WolfHowl : SummoningSkill, IPhaseEnterHandler, IDamagedHandler
{
    public WolfHowl() : base("Wolf Howl") { }
    protected override void Envoke()
    {
        Summon("WolfCub");
        Summon("WolfMother");
        Summon("Wolf");
        Summon("Wolf");
        // TODO: Limited 2 — on game start and first time HP < 5: summon 幼狼1 狼母1 狼*2
    }
    public void OnPhase(Phase phase)
    {
        //保留一个Limitied给OnDamaged
        if (phase is StartPhase && LimitedTimes > 1)
            CheckAndEvoke();
    }
    public void OnDamaged(Player attacker, Player victim, int damage, out int finalDamage)
    {
        if (Owner.status.HP.Value <= 5)
            Envoke();
        finalDamage = damage;
    }
}

public class CloneTechnique : EnemySkill, IPhaseEnterHandler
{
    public CloneTechnique() : base("Clone Technique") { }
    protected override void Envoke()
    {
        // TODO: From round 3 every 6 rounds split into 3 clones 3 turns, then merge
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class Nurture : EnemySkill, IPhaseEnterHandler
{
    public Nurture() : base("Nurture") { }
    protected override void Envoke()
    {
        // TODO: Third round after entry, grow into 狼
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class Overconfidence : EnemySkill, IPhaseEnterHandler
{
    public Overconfidence() : base("Overconfidence") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
        // TODO: Every 2 rounds extra 挑衅 in execute phase to all enemies
    }
}

#endregion

#region On-damage / On-combat

public class FertileBlessing : EnemySkill, IDamagingHandler
{
    public FertileBlessing() : base("Fertile Blessing") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = damage;
        Owner.status.buffs.Apply(new DamageShield(1, Owner));
    }
}

public class ReturnToEarth : EnemySkill, IDamagingHandler
{
    public ReturnToEarth() : base("Return to Earth") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        if (victim.status.HP.Value < victim.status.MaxHP / 3)
        {
            finalDamage = victim.status.HP.Value;
        }
        finalDamage = damage;
    }
}

public class Miasma : EnemySkill, IPhaseEnterHandler, IDamagingHandler, ICombatHandler
{
    public Miasma() : base("Miasma") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
        // TODO: Round 3 apply 尸毒 to all enemies; turn end deal n damage to units with n 尸毒
    }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: On damage add 尸毒; when 尸毒 target attacks owner and no damage, add mark; when they damage owner lose all marks
    }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Apply 尸毒 logic
    }
}

public class Slayer : EnemySkill, IDamagingHandler
{
    public Slayer() : base("Slayer") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: When owner damages enemy and victim HP <= n/3 (n = resource diff), execute
    }
}

public class Encore : EnemySkill, IDamagedHandler, IDamagingHandler
{
    public Encore() : base("Encore") { }
    protected override void Envoke() { }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: Store last damage taken
    }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: Owner's damage + n (n = last damage taken)
    }
}

public class CriticalStrikeEnemy : EnemySkill, ICombatHandler
{
    public CriticalStrikeEnemy() : base("Critical Strike") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: When sword attack hits, next attack base atk 10, ignore def/reflect; if target used 挑衅 then 0 damage
    }
}

public class SwordMomentum : EnemySkill, ICombatHandler, IDamagedHandler
{
    public SwordMomentum() : base("Sword Momentum") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Per damage stack 剑势; each stack atk+1, sword can extra execute once
    }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: On owner damaged clear 剑势
    }
}

public class Unattainable : EnemySkill, ICombatHandler
{
    public Unattainable() : base("Unattainable") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: If last 3 attacks dealt 0 damage to enemy, stun owner
    }
}

public class MagicBullet : EnemySkill, ICombatHandler
{
    public MagicBullet() : base("Magic Bullet") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Start with 6 魔弹; bullet attack consumes 1; if hit enemy execute
    }
}

public class DemonicRite : EnemySkill, ICombatHandler
{
    public DemonicRite() : base("Demonic Rite") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: 7n-th bullet attack has 恶魔祭品: hit → +6 魔弹; reflect → reflect damage = half current HP (ceil)
    }
}

public class DivineBlade : EnemySkill, ICombatHandler
{
    public DivineBlade() : base("Divine Blade") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Sword attack damage * n (n = swords consumed in that attack)
    }
}

public class SharedFate : EnemySkill, IDamagedHandler
{
    public SharedFate() : base("Shared Fate") { }
    protected override void Envoke() { }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: When ally takes damage, halve (floor) and owner takes same amount
    }
}

public class Benediction : EnemySkill, IDamagingHandler
{
    public Benediction() : base("Benediction") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: When 干将/莫邪 deal damage, owner heal equal amount
    }
}

public class Courtship : EnemySkill, IDamagingHandler
{
    public Courtship() : base("Courtship") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: When owner deals damage, can take 3 resources from target (bullet first)
    }
}

public class DesperateStruggle : EnemySkill, IPhaseEnterHandler, IDamagedHandler
{
    public DesperateStruggle() : base("Desperate Struggle") { }
    protected override void Envoke() { }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: When owner HP < 20 lose 地狱集结 gain 你死我活
    }
}

public class LifeOrDeath : EnemySkill, IDamagingHandler
{
    public LifeOrDeath() : base("Life or Death") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: On damage owner loses half HP (floor), extra 2n damage (n = HP lost)
    }
}

public class DeceptiveTrick : EnemySkill, ICombatHandler
{
    public DeceptiveTrick() : base("Deceptive Trick") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Each time enemy hero attacks, owner damage +1
    }
}

public class RampageStomp : EnemySkill, ICombatHandler
{
    public RampageStomp() : base("Rampage Stomp") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Each time enemy hero defends, owner damage +1
    }
}

public class FlyingFlower : EnemySkill, IDamagingHandler
{
    public FlyingFlower() : base("Flying Flower") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: Per damage get 飞花 mark, summon 剑冢 (max 8); each 飞花 atk+0.5
    }
}

public class Emberize : EnemySkill, IDamagingHandler
{
    public Emberize() : base("Emberize") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: After owner damages a unit, gain one bullet from them
    }
}

public class SilentKill : EnemySkill, IDamagingHandler
{
    public SilentKill() : base("Silent Kill") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: n-th damage adds n 凶 marks; each turn lose n resources and one 凶 (n = marks)
    }
}

//这里需要改！回复最终造成的伤害，需要重整一下伤害接口
public class Bloodlust : EnemySkill, IDamagingHandler
{
    public Bloodlust() : base("Bloodlust") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = damage;
        attacker.status.HP.Heal(damage);
    }
}

public class MountainCrusher : EnemySkill, ICombatHandler
{
    public MountainCrusher() : base("Mountain Crusher") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Owner's damage * 3
    }
}

public class Venom : EnemySkill, ICombatHandler
{
    public Venom() : base("Venom") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Attack that deals damage applies 1 中毒; 中毒: 1 damage/turn, -1 layer/turn
    }
}

public class TwoHeads : EnemySkill, ICombatHandler
{
    public TwoHeads() : base("Two Heads") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Owner's each attack resolves twice
    }
}

public class ClearMirror : EnemySkill, ICombatHandler
{
    public ClearMirror() : base("Clear Mirror") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Owner's defense has reflect
    }
}

public class ScorchedEarth : EnemySkill, ICombatHandler
{
    public ScorchedEarth() : base("Scorched Earth") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: When owner attacks, all other characters' supply invalid this turn
    }
}

public class MephistosPact : EnemySkill, IDamagedHandler
{
    public MephistosPact() : base("Mephisto's Pact") { }
    protected override void Envoke() { }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: Owner takes at most 1 damage per turn
    }
}

public class Revival : EnemySkill, IDamagedHandler
{
    public Revival() : base("Revival") { }
    protected override void Envoke() { }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: Each time owner takes damage, owner's damage +1
    }
}

public class Ferocity : EnemySkill, ICombatHandler
{
    public Ferocity() : base("Ferocity") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Owner's damage * 2
    }
}

public class AgileApe : EnemySkill, ICombatHandler
{
    public AgileApe() : base("Agile Ape") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Owner's damage * 2
    }
}

public class OminousFeather : EnemySkill, IDamagingHandler
{
    public OminousFeather() : base("Ominous Feather") { }
    protected override void Envoke() { }
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage)
    {
        finalDamage = 0;
        // TODO: On damage add one 凶 to target; each turn lose n resources and one 凶 (n = 凶 count)
    }
}

public class Relentless : EnemySkill, ICombatHandler
{
    public Relentless() : base("Relentless") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: High attack desire; on hit return consumed resources
    }
}

public class Inviolable : EnemySkill, ICombatHandler
{
    public Inviolable() : base("Inviolable") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: When totem is destroyed, enter high attack desire
    }
}

public class WolfGrudge : EnemySkill, ICombatHandler, IDamagedHandler
{
    public WolfGrudge() : base("Wolf Grudge") { }
    protected override void Envoke() { }
    public void OnCombatEvent(CombatEvent combatEvent)
    {
        // TODO: Damage stacks 复仇; on damage -1 复仇; 幼狼 death +3 复仇, other wolf death +2; damage +1 per 复仇; has 复仇 = high attack desire
    }
    public void OnDamaged(Player attacker, Player victim, int damage, out int blockDamage)
    {
        blockDamage = 0;
        // TODO: On damaged add 复仇
    }
}

#endregion

#region Start / passive / modifier

public class Magnate : EnemySkill, IPhaseEnterHandler
{
    public Magnate() : base("Magnate") { }
    protected override void Envoke()
    {
        // TODO: Game start — bullet 20, sword 10
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class WellEquipped : EnemySkill, IPhaseEnterHandler
{
    public WellEquipped() : base("Well Equipped") { }
    protected override void Envoke()
    {
        // TODO: Game start — bullet 5, sword 5
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class Fabrication : EnemySkill, IPhaseEnterHandler
{
    public Fabrication() : base("Fabrication") { }
    protected override void Envoke()
    {
        // TODO: Game start — n damage shields (n = player equipment count)
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class PotionStockpiling : EnemySkill, IPhaseEnterHandler
{
    public PotionStockpiling() : base("Potion Stockpiling") { }
    protected override void Envoke()
    {
        // TODO: Game start — n bullet, n sword (n = player consumable count)
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class ArmorForging : EnemySkill, IPhaseEnterHandler
{
    public ArmorForging() : base("Armor Forging") { }
    protected override void Envoke()
    {
        // TODO: Every 3 rounds give all other allies n armor (n = player equipment count)
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class BladeForging : EnemySkill, IPhaseEnterHandler
{
    public BladeForging() : base("Blade Forging") { }
    protected override void Envoke()
    {
        // TODO: Every 3 rounds all other allies next attack damage +1
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class PotionRationing : EnemySkill, IPhaseEnterHandler
{
    public PotionRationing() : base("Potion Rationing") { }
    protected override void Envoke()
    {
        // TODO: Every 3 rounds all other allies get sword 1, bullet 1
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class FlowerFarewell : EnemySkill, IPhaseEnterHandler
{
    public FlowerFarewell() : base("Flower Farewell") { }
    protected override void Envoke()
    {
        // TODO: When owner HP <= 7 destroy all 剑冢, lose all 飞花, per 剑冢 damage+1 injured-1, then lose 1 HP/turn
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class TotemCall : SummoningSkill, IPhaseEnterHandler
{
    public TotemCall() : base("Totem Call") { }
    protected override void Envoke()
    {
        // TODO: Every 3 rounds random totem; low desire if slot free, high if no slot
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

#endregion

#region Death / limited / special

public class DeathSense : EnemySkill, IDeathHandler
{
    public DeathSense() : base("Death Sense") { }
    protected override void Envoke() { }
    public bool OnDeath(Player thisPlayer)
    {
        // TODO: Limited 1 — when would die, can use 逆转决策 once
        return false;
    }
}

public class LettingGo : EnemySkill, IPhaseEnterHandler
{
    public LettingGo() : base("Letting Go") { }
    protected override void Envoke()
    {
        // TODO: Limited 1 — when HP < 10, next attack consume all bullet and (m-6) swords, atk and damage +n (n = consumed)
    }
    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase)
            CheckAndEvoke();
    }
}

public class Rebirth : EnemySkill, IDeathHandler
{
    public Rebirth() : base("Rebirth") { }
    protected override void Envoke() { }
    public bool OnDeath(Player thisPlayer)
    {
        // TODO: On death revive at half HP with 焚 marks equal to revived HP
        return true;
    }
}

public class CicadaShell : EnemySkill, IDeathHandler
{
    public CicadaShell() : base("Cicada Shell") { }
    protected override void Envoke() { }
    public bool OnDeath(Player thisPlayer)
    {
        // TODO: On death revive on soldier statue with most resources, same HP, stun 1 turn
        return true;
    }
}

#endregion

#region Modifier / passive (no phase hook)

public class Mirage : EnemySkill
{
    public Mirage() : base("Mirage") { }
    protected override void Envoke()
    {
        // TODO: When using 过来 take at most 5 damage
    }
}

public class TemperedEdge : EnemySkill
{
    public TemperedEdge() : base("Tempered Edge") { }
    protected override void Envoke()
    {
        // TODO: Sword attack can target all enemies, atk +1
    }
}

public class Vengeance : EnemySkill, IDeathHandler
{
    public Vengeance() : base("Vengeance") { }
    protected override void Envoke() { }
    public bool OnDeath(Player thisPlayer)
    {
        // TODO: When both 干将 and 莫邪 dead, owner loses 庇佑 gains 神锋 and 淬刃
        return false;
    }
}

public class FallenStriver : EnemySkill, IPhaseExitHandler
{
    public FallenStriver() : base("Fallen Striver") { }
    protected override void Envoke() { }
    public void ExitingPhase(Phase phase)
    {
        // TODO: Majority supply → next turn 寻欢; majority attack → 好斗; majority defend → 安逸; majority special → 求爱; tie → 求知
    }
}

public class PleasureSeeking : EnemySkill
{
    public PleasureSeeking() : base("Pleasure Seeking") { }
    protected override void Envoke()
    {
        // TODO: Supply resource * 3
    }
}

public class Combativeness : EnemySkill
{
    public Combativeness() : base("Combativeness") { }
    protected override void Envoke()
    {
        // TODO: Damage * 3, atk +1.5
    }
}

public class Ease : EnemySkill
{
    public Ease() : base("Ease") { }
    protected override void Envoke()
    {
        // TODO: Can use 2 defenses; if so, count as extra 过来
    }
}

public class KnowledgeSeeking : EnemySkill
{
    public KnowledgeSeeking() : base("Knowledge Seeking") { }
    protected override void Envoke()
    {
        // TODO: Other skills' base values +1
    }
}

public class Silence : EnemySkill
{
    public Silence() : base("Silence") { }
    protected override void Envoke()
    {
        // TODO: Cannot supply and attack
    }
}

public class Devotion : EnemySkill
{
    public Devotion() : base("Devotion") { }
    protected override void Envoke()
    {
        // TODO: Totem buffs x2
    }
}

public class Tempering : EnemySkill
{
    public Tempering() : base("Tempering") { }
    protected override void Envoke()
    {
        // TODO: Damage + n (n = player upgrade card count)
    }
}

public class GoldenVault : EnemySkill
{
    public GoldenVault() : base("Golden Vault") { }
    protected override void Envoke()
    {
        // TODO: Lock — infinite resources
    }
}

public class DivinePride : EnemySkill
{
    public DivinePride() : base("Divine Pride") { }
    protected override void Envoke()
    {
        // TODO: Lock — first turn no attack; immune negative; 挑衅 invalid; ignore damage from characters damaged this turn by owner
    }
}

public class MeleeDisability : EnemySkill
{
    public MeleeDisability() : base("Melee Disability") { }
    protected override void Envoke()
    {
        // TODO: Cannot draw sword
    }
}

public class HolySword : EnemySkill
{
    public HolySword() : base("Holy Sword") { }
    protected override void Envoke()
    {
        // TODO: Cannot supply bullet
    }
}

public class SturdyShield : EnemySkill
{
    public SturdyShield() : base("Sturdy Shield") { }
    protected override void Envoke()
    {
        // TODO: Low attack desire; defense applies to all allies
    }
}
#endregion