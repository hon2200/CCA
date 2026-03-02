using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using AYellowpaper;

//行动数据库类
//由于在数据库内的行动和玩家使用的行动都是ActionDefine类，需要将ActionDefine类保护起来，怎么办呢？
public class ActionDataBase : MonoSingleton<ActionDataBase>
{
    private void Awake()
    {
        LoadingActions(); 
    }
    // 行动字典，包含所有行动
    public Dictionary<string, ActionDefine> ActionDictionary { get; private set; }
    public Dictionary<(string, string), CounterMethod> VersusTable { get; private set; }
    //读入所有行动
    public void LoadingActions()
    {
        var primitiveActionDictionary = LoadingAction.Instance.CreateAllActionDictionary();
        ActionDictionary = new();
        foreach(var action in primitiveActionDictionary)
        {
            ActionDictionary.Add(action.Key, ActionFactory.Create(action.Key, primitiveActionDictionary));
        }
        VersusTable = LoadingAction.Instance.LoadingVersusTable("../Versus/Attack_Defend_Counter_2dArray.json");
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(ActionDictionary,"Log/Loading/ActionDictionary.txt");
        MyLog.PrintLoadedDictionary(VersusTable,"Log/Loading/VersusTable.txt");
    }
    public Dictionary<string,T> GetActionType<T>()
    {
        Dictionary<string, T> TActionDictionary = new();
        foreach(var action in ActionDictionary)
        {
            if (action.Value is T Taction)
                TActionDictionary.Add(action.Key, Taction);
        }
        return TActionDictionary;
    }

}

