using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ResolutionPhase : Phase
{
    public override void OnEnteringPhase()
    {
        PrintEvent.Instance.LogAction();
        Resolution();
        //PrintResult_Debug();
        PrintEvent.Instance.PrintResult();
        EffectManager.Instance.PlayAll();
    }
    public override void OnExitingPhase()
    {
        ClearPossibleKillers();
    }
    public void Resolution()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.CoolDown();
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Provoke();
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Comeon();
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Supply();
            player.Attack();
        }

        KnockofDeath();
        CheckofDeath();
        CheckofVictory();

        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            var buffSnapShot = player.status.buffs.ToList();
            foreach (var buff in buffSnapShot)
            {
                if (buff is IResolutionHandler resolutionBuff)
                    resolutionBuff.AfterResolution();
                buff.Fade();
                PrintEvent.Instance.log += $"{player.Name}??????{buff.Value}??{buff.ID}\n";
            }
        }

        //PrintResult_Debug();
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
                    if (skill is IDeathHandler deathSkill)
                    {
                        if (deathSkill.OnDeath(player))
                        {
                            if (player.status.life.Value != LifeStatus.EdgeofDeath)
                                reallyDie = false;
                        }
                    }
                }
                foreach(var relic in RougeManager.Instance.rougePlayer.Relics)
                {
                    if(relic is IDeathHandler deathRelic)
                    {
                        if (deathRelic.OnDeath(player))
                        {
                            if (player.status.life.Value != LifeStatus.EdgeofDeath)
                                reallyDie = false;
                        }
                    }
                }

                if (reallyDie)
                {
                    int reward = HeadGain(player.status.MaxHP);

                    foreach (var killerID in player.possibleKillers)
                    {
                        if (PlayerManager.Instance.Players.TryGetValue(killerID, out var killer))
                        {
                            killer.status.resources.Bullet.Get(killer,reward);
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
                foreach(var relic in RougeManager.Instance.rougePlayer.Relics)
                {
                    if(relic is IBattleEndHandler end)
                    {
                        end.OnBattleEnd(player);
                    }
                }
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
        foreach (var player in PlayerManager.Instance.Players.Values)
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

    public void ResolutionCallSkills()
    {
        var playerSnapShot = PlayerManager.Instance.Players.Values.ToList();
        foreach (var player in playerSnapShot)
        {
            foreach (var skill in player.hero.skills)
            {
                if (skill is IResolutionHandler phasedSkill)
                {
                    phasedSkill.AfterResolution();
                }
            }
        }
    }
}
