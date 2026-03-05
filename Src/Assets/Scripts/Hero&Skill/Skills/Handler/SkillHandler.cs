using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region PhaseBasedHandler
public interface IPhaseEnterHandler
{
    public void OnPhase(Phase phase);
}

public interface IPhaseExitHandler
{
    public void ExitingPhase(Phase phase);
}

public interface IResolutionHandler
{
    public void AfterResolution();
}

#endregion

#region CombatHandler
public enum CombatEventType
{
    Attacking,
    Attacked,
    AttackOverwhelming,
    AttackOverwhelmed,
    AttackTakeEffect,
}

public class CombatEvent
{
    public CombatEventType Type;
    public Player Attacker;
    public Player Victim;
    public AttackDefine Attack;
    public CombatEvent(CombatEventType type, Player attacker, Player victim, AttackDefine attack)
    {
        Type = type;
        Attacker = attacker;
        Victim = victim;
        Attack = attack;
    }
}

public interface ICombatHandler
{
    public void OnCombatEvent(CombatEvent combatEvent);
}

public static class CombatDispatcher
{
    //Every time an event happens, broadcast this to ALL PLAYERS who have a
    //combat handler, ask them whether they have a responding skill
    public static void Dispatch(CombatEvent combatEvent, Player thisPlayer)
    {
        foreach (var skill in thisPlayer.hero.skills)
        {
            if (skill is ICombatHandler handler)
            {
                handler.OnCombatEvent(combatEvent);
            }
        }
    }
}

#endregion

public interface IDeathHandler
{
    //return true if reviving
    public bool OnDeath(Player thisPlayer);
}

public interface IDamagingHandler
{
    public void OnDamaging(Player attacker, Player victim, int damage, out int finalDamage);
}

public interface IDamagedHandler
{
    public void OnDamaged(Player attacker, Player victim, int damage, out int finalDamage);
}

public interface IStunningHandler
{
    public void OnStunning(Player attacker, Player victim);
}

public interface IStunnedHandler
{
    public void OnStunned(Player attacker, Player victim);
}

public interface IActionModifier
{
    public void ModifyAction(Player player);
}

/// <summary>Called when a player executes at least one supply action in resolution.</summary>
public interface ISupplyHandler
{
    public void OnSupplied(Player supplier);
}

/// <summary>Called when a player gets a kill (victim died and killer is in possibleKillers).</summary>
public interface IOnKillHandler
{
    public void OnKill(Player killer, Player victim);
}


# region OutofBattleHandler
public interface IBattleEndHandler
{
    public void OnBattleEnd(Player player);
}

#endregion