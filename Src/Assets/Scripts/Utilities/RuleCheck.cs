using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RuleCheck
{
    //这个函数负责检查行动（攻击行动的Cost不受目标影响-暂时无法处理激光炮）是否合法
    public static bool isActionLegal(this Player player,ActionDefine actionDefine)
    {
        if (player.status.resources.Bullet.Value - actionDefine.Costs[0] < 0)
            return false;
        if (player.status.resources.AvailableSword.Value - actionDefine.Costs[1] < 0)
            return false;
        //目前过来和挑衅单独判定，之后会把CD对行动合法性的印象单独写出来
        if (actionDefine.Name == "Comeon")
        {
            if (BattleManager.Instance.Turn.Value == 1)
                return false;
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            foreach (var action in actions)
            {
                if (action.ActionInfo.ID == "Comeon")
                    return false;
            }
        }
        if (actionDefine.Name == "Provoke")
        {
            if (BattleManager.Instance.Turn.Value == 1)
                return false;
            player.action.LongHistory.TryGetValue((BattleManager.Instance.Turn.Value - 1, false),
                out var actions);
            foreach (var action in actions)
            {
                if (action.ActionInfo.ID == "Provoke")
                    return false;
            }
        }
        return true;
    }
}
