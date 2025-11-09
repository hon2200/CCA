using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProtectingAlly : PhasebasedSkill
{
    public ProtectingAlly()
    {
        ID = "Save Me";
    }
    public override void AfterResolution(Player thisPlayer)
    {
        if(thisPlayer.status.life.Value == LifeStatus.Death)
            BattleManager.Instance.OnDefeated.Invoke();
    }
}
