using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ResolutionPhase : Singleton<ResolutionPhase>, Phase
{
    public void OnEnteringPhase()
    {
        EventPanelLogic.Instance.OpenEventPanel();
        PrintEvent.Instance.ClearText();
        PrintEvent.Instance.LogAction();
        Resolution();
        PrintResult_Debug();
        PrintEvent.Instance.PrintResult();
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {
        ClearPossibleKillers();
    }
    public void Resolution()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.ConsumeAndCD();
            player.CoolDown();
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Provoke();
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.Comeon();
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.Supply();
            player.Attack();
        }

        KnockofDeath();
        CheckofDeath();
        CheckofVictory();

        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            foreach(var skill in player.hero.skills)
            {
                if(skill is PhasebasedSkill phasebased)
                {
                    phasebased.AfterResolution(player);
                }
            }
        }

        PrintResult_Debug();
    }
    public void PrintResult_Debug()
    {
        if (BattleManager.Instance.Turn.Value == 1)
        {
            MyLog.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players, 
                new string[] {"ID_inGame", "status"},"Log/InGame/PlayerStatus.txt");
            MyLog.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action", "Log/InGame/PlayerAction.txt");
        }
        else
        {
            MyLog.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players,
                new string[] { "ID_inGame", "status" }, "Log/InGame/PlayerStatus.txt", false);
            MyLog.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action", "Log/InGame/PlayerAction.txt",false);
        }
    }
    public void KnockofDeath()
    {
        var playersSnapshot = PlayerManager.Instance.Players.Values.ToList();

        foreach (var player in playersSnapshot)
        {
            if (player.status.life.Value == LifeStatus.EdgeofDeath)
            {
                bool reallyDie = true;

                var skillsSnapshot = player.hero.skills.ToList();
                foreach (var skill in skillsSnapshot)
                {
                    if (skill is TriggerSkill triggered)
                    {
                        if (triggered.OnDeath(player))
                            reallyDie = false;
                    }
                }

                if (reallyDie)
                {
                    int reward = HeadGain(player.status.MaxHP);

                    foreach (var killerID in player.possibleKillers)
                    {
                        if (PlayerManager.Instance.Players.TryGetValue(killerID, out var killer))
                        {
                            killer.status.resources.Bullet.Get(reward);
                        }
                    }

                    player.status.life.DieOut();
                    PrintEvent.Instance.LogKiller(player, reward);
                }
            }
        }
    }
    public void CheckofDeath()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            if (player.status.life.Value == LifeStatus.Death && player.playerType == PlayerType.Human)
            {
                BattleManager.Instance.OnDefeated.Invoke();
            }
        }
    }
    public void CheckofVictory()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            if (player.status.life.Value == LifeStatus.Alive && player is AIPlayer aIPlayer && !aIPlayer.isFriend)
            {
                return;
            }
        }
        BattleManager.Instance.OnWinning.Invoke();
    }

    public void ClearPossibleKillers()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.possibleKillers.Clear();
        }
    }
    
    private int HeadGain(int HP)
    {
        if (HP == 1)
            return 1;
        return HP / 5 + 2;
    }
}
