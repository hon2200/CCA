using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IsAliveAttribute : ObservableAttribute<LifeStatus>
{
    public void Born() => SetValue(LifeStatus.Alive, "Born");
    public void Dying() => SetValue(LifeStatus.EdgeofDeath, "Dying");
    public void DieOut()
    {
        PlayerManager.Instance.AlivePlayerNumber--;
        SetValue(LifeStatus.Death, "Dieout");
    }
}

public enum LifeStatus
{
    Alive = 1,
    EdgeofDeath = 2,
    Death = 3,
}
