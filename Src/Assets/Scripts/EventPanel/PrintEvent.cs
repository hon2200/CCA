using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrintEvent : MonoSingleton<PrintEvent>
{
    public string log;
    public TextMeshPro Text;
    public void LogKiller(Player victim, int reward)
    {
        log += (victim.Name + victim.ID_inGame + "死啦" + "\n");
        foreach (var killerID in victim.possibleKillers)
        {
            PlayerManager.Instance.Players.TryGetValue(killerID, out var killer);
            log += (killer.Name + killer.ID_inGame + "拿到" + reward + " 子弹" + "\n");
        }
    }
    public void LogProvoke(Player victim, Player provoker, int HPlost)
    {
        log += (victim.Name + victim.ID_inGame + " 掉了 " + HPlost +
            " HP因为" + provoker.Name + provoker.ID_inGame + "挑衅未响应" + "\n");
    }
    public void LogComeon(Player comeoner, Player beComeoner, ActionDefine action)
    {
        log += (comeoner.name + comeoner.ID_inGame + "过来了" + 
            beComeoner.Name + beComeoner.ID_inGame + "的 " + action.Name + "\n");
    }
    //Log the damage dealt

    public void LogDamage(Player attacker, Player victim, int damage)
    {
        log += (attacker.Name + attacker.ID_inGame + "对" +
            victim.Name + victim.ID_inGame + "造成" + damage + "伤害" +"\n");
    }
    public void LogAction()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            foreach (var action in player.action)
            {
                switch(action.TargetType)
                {
                    case TargetType.Self:
                        log += (player.Name + player.ID_inGame + "使用" + action.Name + "\n");
                        break;
                    case TargetType.Enemy:
                        PlayerManager.Instance.Players.TryGetValue(action.Target, out var target);
                        log += (player.Name + player.ID_inGame + "对" +
                            target.Name + target.ID_inGame + "使用" + action.Name + "\n");
                        break;
                }
            }
        }
        log += "\n";
    }
    public void PrintResult()
    {
        Text.text = log;

    }

    public void ClearText()
    {
        Text.text = "";
        log = "";
    }
}
