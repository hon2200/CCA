using Common.Data;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class BalanceCount : MonoBehaviour
{ 
    public UICreatRoom UICreatRoom;
    public CharacterSet characterSet;
    public Transform Root;

    public TMP_Dropdown Rule;


    public TMP_InputField NumberLimit;
    public int limit;

    private List<CharacterSet> characterSets = new List<CharacterSet>();

    private readonly Dictionary<int, int> modeConfig = new Dictionary<int, int>()
    {
        {0, 2},  // ���˶�ս
        {1, 4},  // �ŶӾ���
        {2, 4}   // ��սģʽ
    };


    void Start()
    {
        Rule.onValueChanged.AddListener(Init);
        NumberLimit.onValueChanged.AddListener(LimitNumber);
        Init(0);
    }

    void Init(int mode)
    {
        // ������н�ɫ��
        foreach (Transform child in Root)
        {
            Destroy(child.gameObject);
        }
        characterSets.Clear();
        // ������ʼ��ɫ��
        limit = modeConfig[mode];
        NumberLimit.text = limit.ToString();
        for (int i = 0; i < modeConfig[mode]; i++)
        {
            AddCharacterSet();
        }
    }

    public void AddCharacterSet()
    {
        int total = characterSets.Sum(s => s.CurrentCount) + 1;
        if (total > limit)
        {
            MessageBox.Show($"���������ܳ����������� ��{limit}��", "����", MessageBoxType.Error, "ȷ��", "ȡ��");
            return;
        }
        CharacterSet NewSet = Instantiate(characterSet, Root);
        NewSet.CurrentCount = 1;
        NewSet.Count.onValueChanged.AddListener(_ => OnCountChanged(NewSet));
        NewSet.Count.onEndEdit.AddListener(_ => OnEndEdit(NewSet));
        NewSet.HP.onValueChanged.AddListener((input) => NewSet.HP.text = input);
        NewSet.Bullets.onValueChanged.AddListener((input) => NewSet.Bullets.text = input);
        NewSet.Swords.onValueChanged.AddListener((input) => NewSet.Swords.text = input);

        characterSets.Add(NewSet);
    }

    public void RemoveCharacterSet()
    {
        if (characterSets.Count <= 1) { MessageBox.Show("���ٴ���һ֧���飡", "����", MessageBoxType.Error, "ȷ��", "ȡ��"); return; }
       

        var lastSet = characterSets.Last();
        int removedCount = lastSet.CurrentCount;
        lastSet.Count.onValueChanged.RemoveAllListeners();
        Destroy(lastSet.gameObject);
        characterSets.Remove(lastSet);

        var maxSet = characterSets.OrderByDescending(s => s.CurrentCount).First();
        int newCount = maxSet.CurrentCount + removedCount;
        maxSet.CurrentCount = newCount;

    }

    private void LimitNumber(string arg0)
    {
        NumberLimit.onValueChanged.RemoveAllListeners();
        int lastlimit = int.Parse(NumberLimit.text);
        if (!int.TryParse(arg0, out limit))
        {
            MessageBox.Show("�������ݱ���Ϊ���֣�", "����", MessageBoxType.Error, "ȷ��", "ȡ��");
            NumberLimit.text = lastlimit.ToString();
            limit = lastlimit;
            return;
        }
        if (limit < 2 || limit > 10)
        {
            MessageBox.Show("�������Ϸ�", "����", MessageBoxType.Error, "ȷ��", "ȡ��");
            NumberLimit.text = lastlimit.ToString();
            limit = lastlimit;
            return;
        }
        limit =int.Parse(NumberLimit.text);
        NumberLimit.onValueChanged.AddListener(LimitNumber);
    }

    void OnCountChanged(CharacterSet newSet)
    {
        if (int.TryParse(newSet.Count.text, out int newCount))
        {
            if (newCount < 1) { MessageBox.Show("��������С��1��", "����", MessageBoxType.Error, "ȷ��", "ȡ��"); ResetInputField(newSet); return; }
            int oldCount = newSet.CurrentCount;
            int total = characterSets.Sum(s => s.CurrentCount) - oldCount + newCount;
            if (total > limit) { MessageBox.Show($"���������ܳ����������� ��{limit}��", "����", MessageBoxType.Error, "ȷ��", "ȡ��"); ResetInputField(newSet); return; }
            newSet.CurrentCount = newCount;
        }
        else
        {
            ResetInputField(newSet);
        }
    }
    void OnEndEdit(CharacterSet newSet)
    {
        ResetInputField(newSet);
    }
    private void ResetInputField(CharacterSet set)
    {
        // �����¼�ѭ������
        set.Count.onValueChanged.RemoveListener(_ => OnCountChanged(set));
        set.Count.text = set.CurrentCount.ToString();
        set.Count.onValueChanged.AddListener(_ => OnCountChanged(set));
    }
    public void SubmitAllCharacters()
    {
        
        int total = characterSets.Sum(s => s.CurrentCount);
        if (total != limit) {
            MessageBox.Show("�뽫����������", "����", MessageBoxType.Error, "ȷ��","ȡ��");
            return;
        }
        foreach (var set in characterSets)
        {
            if (!int.TryParse(set.HP.text, out int hp) ||
                !int.TryParse(set.Bullets.text, out int bullets) ||
                !int.TryParse(set.Swords.text, out int swords))
            {
                MessageBox.Show("�������ݱ���Ϊ���֣�", "����", MessageBoxType.Error, "ȷ��", "ȡ��");
                return;
            }
        }
        UICreatRoom.CreatRoom(characterSets, limit);
    }

    
}