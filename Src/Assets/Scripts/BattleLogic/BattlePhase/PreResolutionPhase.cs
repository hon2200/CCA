using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PreResolutionPhase : Phase
{
    public void OnEnteringPhase()
    {
        PreResolution();
    }
    public void OnExitingPhase()
    {

    }
    public void PreResolution()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.GiveValueToLaserCannon();
            foreach(var skill in player.hero.skills)
            {
                if(skill is PhasebasedSkill phasebasedSkill)
                {
                    phasebasedSkill.BeforeResolution();
                }
            }
        }
    }
}
