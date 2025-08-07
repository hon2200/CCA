using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//�غϽ���׶νű�
//����ű����ڵ����⣺����������Player����ڲ��ṹ��һ��Player�෢���ı䣬�����������������ö���Ҫ������
//�ؼ����������ⲻֹ������ű����棬�����ϵ������У�Player���ڲ���Status��ActionҲ����������ϡ�
//��ô���أ�
//����Ѵ���ֿ���һ�㣬��Player������ר�ŷ�һ��������������ز������������������Ĺ��ܾͻ�򵥺ܶ࣬
//��Ϊ���˽���׶Σ�һЩ�����ط�Ҳ���õ����㺯����������һ
//Player���Լ��ĺ����������Լ��ڲ��ṹ�����û���κ����⣬������������ⲿ�������������
public class ResolutionPhase : Singleton<ResolutionPhase>, Phase
{
    public void OnEnteringPhase()
    {
        EventPanelLogic.Instance.OpenEventPanel();
        Resolution();
        PrintEvent.Instance.ClearText();
        PrintEvent.Instance.PrintText();
        PrintResult_Debug();
        //����׶β��ȴ����
        BattleManager.Instance.PhaseAdvance();
    }
    public void OnExitingPhase()
    {

    }
    //����׶Σ������ҵ��ж�
    //����Ŀǰ��û�������buff�����Խ���׶ξ���д������
    public void Resolution()
    {
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Consume();
            player.CoolDownSword();
        }
        foreach (var player in PlayerManager.Instance.Players.Values)
        {
            player.Provoke();
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.Comeon();
        }
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            player.Supply();
            player.Attack();
        }
        PrintResult_Debug();
    }
    public void PrintResult_Debug()
    {
        if (BattleManager.Instance.Turn.Value == 1)
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players, 
                new string[] {"ID_inGame", "status"},"Log/InGame/PlayerStatus.txt");
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action", "Log/InGame/PlayerAction.txt");
        }
        else
        {
            Log.PrintSpecificPropertiesInDictionary(PlayerManager.Instance.Players,
                new string[] { "ID_inGame", "status" }, "Log/InGame/PlayerStatus.txt", false);
            Log.PrintNestedPropertyInDictionary(PlayerManager.Instance.Players,
                "action", "Log/InGame/PlayerAction.txt",false);
        }

    }
}
