using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrintEvent : MonoSingleton<PrintEvent>
{
    public TextMeshPro Text;
    public void PrintText()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            foreach (var action in player.action)
            {
                Text.text += ("玩家" + player.ID_inGame + "对" + action.Target + "使用了" + action.Name);
                Text.text += "\n";
                if(action is AttackDefine attack)
                {
                    Text.text += ("玩家" + player.ID_inGame + "对" + attack.Victim + "造成了" + attack.Damage + "伤害");
                    Text.text += "\n";
                }
            }
           
        }
    }
    public void ClearText()
    {
        Text.text = "";
    }
}
