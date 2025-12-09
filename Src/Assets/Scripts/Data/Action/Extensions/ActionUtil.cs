using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ActionUtil
{
    public static bool IsAction<Type>(string ID) where Type:ActionDefine
    {
        ActionDataBase.Instance.ActionDictionary.TryGetValue(ID, out var actionDefine);
        if (actionDefine is Type)
            return true;
        return false;
    }
    public static bool IsAttackLight(AttackDefine attack)
    {
        if (attack.ID == "laser_cleave" || attack.ID == "laser_stab" || attack.ID == "laser_cannon" || attack.ID == "laser_shoot")
            return true;
        return false;
    }
}
