using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region PhaseBasedHandler
public interface IPhaseEnterHandler
{
    public void OnPhase(Phase phase, Player player);
}

public interface IPhaseExitHandler
{
    public void ExitingPhase(Phase phase, Player player);
}

public interface IResolutionHandler
{
    public void AfterResolution(Player player);
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
    public void OnDamaging(Player attacker, Player victim, int damage);
}

public interface IDamagedHandler
{
    public void OnDamaged(Player attacker, Player victim, int damage);
}

public interface IActionReplacer
{
    public void ReplaceAction(Player player);
}