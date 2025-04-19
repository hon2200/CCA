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
        {0, 2},  // 单人对战
        {1, 4},  // 团队竞技
        {2, 4}   // 混战模式
    };


    void Start()
    {
        Rule.onValueChanged.AddListener(Init);
        NumberLimit.onValueChanged.AddListener(LimitNumber);
        Init(0);
    }

    void Init(int mode)
    {
        // 清除现有角色组
        foreach (Transform child in Root)
        {
            Destroy(child.gameObject);
        }
        characterSets.Clear();
        // 创建初始角色组
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
            MessageBox.Show($"总人数不能超过总数限制 ：{limit}人", "错误", MessageBoxType.Error, "确定", "取消");
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
        if (characterSets.Count <= 1) { MessageBox.Show("至少存在一支队伍！", "错误", MessageBoxType.Error, "确定", "取消"); return; }
       

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
            MessageBox.Show("输入内容必须为数字！", "错误", MessageBoxType.Error, "确定", "取消");
            NumberLimit.text = lastlimit.ToString();
            limit = lastlimit;
            return;
        }
        if (limit < 2 || limit > 10)
        {
            MessageBox.Show("人数不合法", "错误", MessageBoxType.Error, "确定", "取消");
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
            if (newCount < 1) { MessageBox.Show("人数不能小于1！", "错误", MessageBoxType.Error, "确定", "取消"); ResetInputField(newSet); return; }
            int oldCount = newSet.CurrentCount;
            int total = characterSets.Sum(s => s.CurrentCount) - oldCount + newCount;
            if (total > limit) { MessageBox.Show($"总人数不能超过总数限制 ：{limit}人", "错误", MessageBoxType.Error, "确定", "取消"); ResetInputField(newSet); return; }
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
        // 避免事件循环触发
        set.Count.onValueChanged.RemoveListener(_ => OnCountChanged(set));
        set.Count.text = set.CurrentCount.ToString();
        set.Count.onValueChanged.AddListener(_ => OnCountChanged(set));
    }
    public void SubmitAllCharacters()
    {
        
        int total = characterSets.Sum(s => s.CurrentCount);
        if (total != limit) {
            MessageBox.Show("请将人数填满！", "错误", MessageBoxType.Error, "确定","取消");
            return;
        }
        foreach (var set in characterSets)
        {
            if (!int.TryParse(set.HP.text, out int hp) ||
                !int.TryParse(set.Bullets.text, out int bullets) ||
                !int.TryParse(set.Swords.text, out int swords))
            {
                MessageBox.Show("输入内容必须为数字！", "错误", MessageBoxType.Error, "确定", "取消");
                return;
            }
        }
        UICreatRoom.CreatRoom(characterSets, limit);
    }

    
}