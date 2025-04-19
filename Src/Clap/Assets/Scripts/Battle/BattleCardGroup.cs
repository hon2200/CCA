using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BattleCardGroup : MonoSingleton<BattleCardGroup>
{
    public UIBookmessage message;


    public void UpdayeData(string icon, string name, string description)
    {
        message.UpdateInfo(icon,name,description);
    }
}
