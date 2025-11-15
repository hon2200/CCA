using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;


public class CDManager
{
    public Dictionary<string, int> ActionsinCD { get; private set; }
    public CDManager()
    {
        ActionsinCD = new Dictionary<string, int>();
    }
    public void AddAction(string ID,int duration)
    {
        if (ActionsinCD.ContainsKey(ID))
            return;
        ActionsinCD.Add(ID, duration);
    }
    //This performs cooldown
    public void CoolDown()
    {
        var actionsinNewCD = new Dictionary<string, int>();
        foreach (var actioninCD in ActionsinCD)
        {
            if (actioninCD.Value > 0)
                actionsinNewCD.Add(actioninCD.Key, actioninCD.Value - 1);
        }
        ActionsinCD = actionsinNewCD;
    }
    public void ActionEnterCD(ActionDefine action)
    {
        if (action.CD > 0 && !ActionsinCD.ContainsKey(action.ID))
            ActionsinCD.Add(action.ID, action.CD);
    }
}
