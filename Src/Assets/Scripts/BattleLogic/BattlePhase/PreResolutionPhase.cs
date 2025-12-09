using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PreResolutionPhase : Singleton<PreResolutionPhase>, Phase
{
    public void OnEnteringPhase()
    {
        PreResolution();
        BattleManager.Instance.PhaseAdvance();
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
                    phasebasedSkill.InvokeBeforeResolution(player);
                }
            }
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            foreach (var buff in player.status.buffs)
            {
                buff.BeforeResolution(player);
            }
        }
    }
}
