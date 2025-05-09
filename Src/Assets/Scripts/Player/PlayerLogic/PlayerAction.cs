using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction
{
    //��غ��������ж�������Ŀ���
    public List<ActionBase> ActionList { get; set; }
    //�ж���ʷ//ʹ�����������洢//���Է��ʵ���ʷ�ж�����һϵ��buff������ս������
    public Dictionary<(int, bool), List<ActionBase>> LongHistory { get; set; }
    public PlayerAction()
    {
        ActionList = new();
        LongHistory = new();
    }

    //������ʷ�ж�
    public void ReadinHistory(bool isProcessed)
    {
        List<ActionBase> newHistory_l = new List<ActionBase>();
        foreach (ActionBase action in ActionList)
        {
            //����һ���ж���history
            newHistory_l.Add((ActionBase)action.Clone());
        }
        LongHistory.Add((BattleManager.Instance.Turn.Value, isProcessed), ActionList);
    }
    //ͨ����̬���͹��죬ʵ�ּ��Ư���Ĵ���
    public void ReadinMove(string ID, int Target)
    {
        if (ActionDataBase.Instance.ActionDictionary.TryGetValue(ID, out var value))
        {
            // 1. ��ȡ value ��ʵ�����ͣ��� AttackDefine��
            Type actionDefineType = value.GetType();

            // 2. ���췺�� BattleAction<T> ���ͣ��� BattleAction<AttackDefine>��
            Type actionGenericType = typeof(BattleAction<>).MakeGenericType(actionDefineType);

            // 3. ��̬����ʵ��
            //ע�⣬����ʹ��value�����
            object actionInstance = Activator.CreateInstance(
                actionGenericType,
                (ActionDefine)value.Clone(),
                Target
            );

            // 4. ��ӵ� ActionList��ActionList �� List<ActionBase>��
            ActionList.Add((ActionBase)actionInstance);
        }
        else
        {
            Debug.Assert(false, "Wrong ID");
        }
    }
    public void ClearMove()
    {
        ActionList.Clear();
    }
}
