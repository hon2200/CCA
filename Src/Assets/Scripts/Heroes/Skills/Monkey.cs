using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//在结算阶段前进行的处理
public interface OnPreBuff
{
    public void OnPreBuff(Player player);
}


public class Monkey : OnPreBuff
{
    public void OnPreBuff(Player monkey)
    {
        foreach(var action in monkey.action)
        {
            if(action is AttackDefine attack)
            {
                attack.Damage *= 3;
            }
        }
    }
}

public class Lvbu : OnPreBuff
{
    public void OnPreBuff(Player lvbu)
    {
        foreach (var action in lvbu.action)
        {
            if (action is AttackDefine attack)
            {
                attack.Level += PlayerManager.Instance.AlivePlayerNumber * 0.5f;
            }
        }
    }
}

public class Tom : OnPreBuff
{
    public int MouseMark;
    public void OnPreBuff(Player Tom)
    {
        foreach (var action in Tom.action)
        {
            if (action is AttackDefine attack)
            {
                attack.Level += MouseMark * 0.5f;
            }
        }
    }
}