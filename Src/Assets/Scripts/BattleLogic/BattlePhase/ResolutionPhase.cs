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
        PrintEvent.Instance.PrintAction();
        Resolution();
        PrintEvent.Instance.PrintResult();
        PrintResult_Debug();
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
            player.Consume();
            player.CoolDownSword();
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
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            if (player.status.life.Value == LifeStatus.EdgeofDeath)
            {
                ///
/*                if (player.hero.ID == "Zhongkui")
                {
                 
                    var nukeAction = (AttackDefine)ActionDataBase.Instance.ActionDictionary["nuclear_bomb"];

                    foreach (var victim in PlayerManager.Instance.Players.Values)
                    {
                        if (victim.ID_inGame != player.ID_inGame) // 避免自伤
                        {
                            nukeAction.HowtoAttack(player, victim);
                        }
                    }
                }
                //if是钟馗
                //交互按钮亮起
                //��ťOnclick+(){������˵�}
                //发动大核弹
*/
                foreach(var killerID in player.possibleKillers)
                {
                    PlayerManager.Instance.Players.TryGetValue(killerID, out var killer);
                    killer.status.resources.Bullet.Get(HeadGain(player.status.MaxHP));
                }
                player.status.life.DieOut();
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
        return HP / 5 + 2;
    }
}
