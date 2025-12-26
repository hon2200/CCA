using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//管理各个阶段
public abstract class Phase
{
    public abstract void OnEnteringPhase();
    public abstract void OnExitingPhase();
    public void EnteringCallSkills()
    {
        var playerSnapShot = PlayerManager.Instance.Players.Values.ToList();
        foreach (var player in playerSnapShot)
        {
            foreach (var skill in player.hero.skills)
            {
                if (skill is IPhaseEnterHandler phasedSkill)
                {
                    phasedSkill.OnPhase(this, player);
                }
            }
        }
    }
    public void ExitingCallSkills()
    {
        var playerSnapShot = PlayerManager.Instance.Players.Values.ToList();
        foreach (var player in playerSnapShot)
        {
            foreach (var skill in player.hero.skills)
            {
                if (skill is IPhaseExitHandler phasedSkill)
                {
                    phasedSkill.ExitingPhase(this, player);
                }
            }
        }
    }
}


public enum PhaseName
{
    StartPhase = 1,
    ActionPhase = 2,
    ChasePhase = 3,
    ResolutionPhase = 4,
    EndPhase = 5,
}