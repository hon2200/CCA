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
                Text.text += ("���" + player.ID_inGame + "��" + action.Target + "ʹ����" + action.Name);
                Text.text += "\n";
                if(action is AttackDefine attack)
                {
                    Text.text += ("���" + player.ID_inGame + "��" + attack.Victim + "�����" + attack.Damage + "�˺�");
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
