using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//PlayerAction��Ϣ����

/*
 * ���ȣ���ҵ��ж�������һ��Observable�ɹ۲�����Ҳ����ζ�ţ��κζ����ĸı䶼�ᱻ��⵽���ᷴ��һ��message��
 * ����˵����message����������
 * ���ж��ĸı�Ŀǰֻ������������ܷ�����1.��Ҷ��룬2.���Զ��룬3.ս�����������ɸ��ġ�4.ս��������һ��������ж���
 * ��������ֱ�����ΪPlayer��AI��InGame��End
 * 2.����message�Ĺ淶��Add_Player��AddΪԭ��List�Ĳ����������»������ӣ����߶���д
 * 3.Ҫ����ľ����⼸�����������ϲ�����ί��
 * 4.��������string������ܲ���enum�Ͻ�������enumȷʵҲ�����鷳�ˣ�����string����ͨ����ȡǰ�漸���ֻ��ߺ��漸���ֵķ�ʽ��ȡ��Ϣ��
 * 
 */
public class PlayerAction : ObservableList<ActionBase>
{
    //��غ��������ж�������Ŀ���
    //�ж���ʷ//ʹ�����������洢//���Է��ʵ���ʷ�ж�����һϵ��buff������ս������
    // key: (turnNumber, isProcessed), value: �ûغϵ��ж�����
    public Dictionary<(int, bool), List<ActionBase>> LongHistory { get; } = new();

    // ��ȡ��ǰ�ж�����ʷ��¼
    public void ReadinHistory(bool isProcessed)
    {
        // ��������б�
        var historySnapshot = new List<ActionBase>(this.Count);
        foreach (var action in this) // ֱ�ӱ�����ǰ�б�
        {
            historySnapshot.Add((ActionBase)action.Clone());
        }

        // �� (��ǰ�غ�, �Ƿ��Ѵ���) Ϊ���洢
        LongHistory.Add((BattleManager.Instance.Turn.Value, isProcessed), historySnapshot);
    }
    public PlayerAction()
    {
        LongHistory = new();
    }
    //ͨ����̬���͹��죬ʵ�ּ��Ư���Ĵ���
    public void ReadinMove(string ID, int Target, string Case)
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

            // 4. ��ӵ� Value��Value �� List<ActionBase>��
            Add((ActionBase)actionInstance, "Add_" + Case);
        }
        else
        {
            Debug.Assert(false, "Wrong ID");
        }
    }

    public void DeleteMove(ActionBase actionBase, string Case)
    {
        Remove(actionBase, "Remove_" + Case);
    }

    public void DeleteMoveAt(int order, string Case)
    {
        RemoveAt(order, "Remove_" + Case);
    }

    public void ClearMove(string Case)
    {
        Clear("Clear_" + Case);
    }

}
