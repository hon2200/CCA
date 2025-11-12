using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//Process the actions loaded from Json files
public static class ActionFactory
{
    public static ActionDefine Create(string id, Dictionary<string,ActionDefine> actionDic)
    {
        if (!actionDic.TryGetValue(id, out var baseDef))
            throw new Exception($"Unknown action ID: {id}");

        // 1. Find special subclass type (if any)
        Type t = ActionTypeRegistry.GetTypeById(id);

        // 2. Create instance
        ActionDefine instance = (t != null)
            ? (ActionDefine)Activator.CreateInstance(t)
            : (ActionDefine)Activator.CreateInstance(baseDef.GetType());

        // 3. Copy all fields from JSON-loaded definition
        baseDef.Copy(instance);
        return instance;
    }
}

//You should register all actions.
//Whether they have special types or not
//Things like AttackDefine don't have an ID, thus won't be registered
//Things like NuclearBomb will be registered
public static class ActionTypeRegistry
{
    private static readonly Dictionary<string, Type> registry = new();
    static ActionTypeRegistry()
    {
        var types = typeof(ActionDefine).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ActionDefine)));

        foreach (var type in types)
        {
            var temp = (ActionDefine)Activator.CreateInstance(type);
            if (!string.IsNullOrEmpty(temp.ID))
            {
                registry[temp.ID] = type;
                Debug.Log("Action" + temp.ID + "Created");
            }

        }
    }
    public static Type GetTypeById(string id)
    {
        registry.TryGetValue(id, out var type);
        return type;
    }
}

//加载ActionDefine，是ActionDefine对应的简单工厂，但也承担了一部分“生产哪些”的问题，这个类会将ActionDefine读入为字典
public class LoadingAction : Singleton<LoadingAction>
{
    //关于行动的主文件夹位置
    protected string MainPath { get; private set; }
    public LoadingAction() =>
        MainPath = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/ActionDefine/");
    //创建全部的字典
    public Dictionary<string, ActionDefine> CreateAllActionDictionary()
    {
        var dict = new Dictionary<string, ActionDefine>();
        //对每一个行动，创建字典
        foreach (ActionType type in Enum.GetValues(typeof(ActionType)))
        {
            if (type == ActionType.Origin)
                continue;
            var subDict = CreateSpecificDictionary(type);
            foreach (var kvp in subDict)
                dict.Add(kvp.Key, kvp.Value);
        }
        return dict;
    }
    //创建某一个具体行动类别的字典
    private Dictionary<string, ActionDefine> CreateSpecificDictionary(ActionType actionType)
    {
        string path;
        switch (actionType)
        {
            //BulletAttack帮忙把所有Attack都加载了，就没有SwordAttack什么事儿了
            case ActionType.Attack:
                path = Path.Combine(MainPath, "Attack.json");
                Dictionary<string, AttackDefine> attackDictionary = JsonLoader.DeserializeObject<Dictionary<string, AttackDefine>>(path);
                return attackDictionary.ConvertToParentDictionary<string, ActionDefine, AttackDefine>();
            case ActionType.Defend:
                path = Path.Combine(MainPath, "Defend.json");
                Dictionary<string, DefendDefine> defendDictionary = JsonLoader.DeserializeObject<Dictionary<string, DefendDefine>>(path);
                return defendDictionary.ConvertToParentDictionary<string, ActionDefine, DefendDefine>();
            case ActionType.Counter:
                path = Path.Combine(MainPath, "Counter.json");
                Dictionary<string, CounterDefine> counterDictionary = JsonLoader.DeserializeObject<Dictionary<string, CounterDefine>>(path);
                return counterDictionary.ConvertToParentDictionary<string, ActionDefine, CounterDefine>();
            case ActionType.Supply:
                path = Path.Combine(MainPath, "Supply.json");
                Dictionary<string, SupplyDefine> supplyDictionary = JsonLoader.DeserializeObject<Dictionary<string, SupplyDefine>>(path);
                return supplyDictionary.ConvertToParentDictionary<string, ActionDefine, SupplyDefine>();
            case ActionType.Special:
                path = Path.Combine(MainPath, "Special.json");
                Dictionary<string, SpecialDefine> specialDictionary = JsonLoader.DeserializeObject<Dictionary<string, SpecialDefine>>(path);
                return specialDictionary.ConvertToParentDictionary<string, ActionDefine, SpecialDefine>();
            default:
                Debug.Assert(false, "Wrong BattleAction ActionType");
                return null;
        }
    }
    //攻击-防御反制行动表的读取
    public Dictionary<(string, string), CounterMethod> LoadingVersusTable(string path)
    {
        Dictionary<(string, string), CounterMethod> dict = new();
        string fullPath = Path.Combine(MainPath, path);
        //初始化攻击防御对应表
        string[][] array2D = JsonLoader.DeserializeObject<string[][]>(fullPath);
        //Array2D的结构，[0][0]放空，excel中的列，[i][0]存放各个攻击的ID，
        //excel中的行，[0][i]存放各个防御或反制的ID
        /*e.g.
            * [Null,301,302,303,304]
            * [201,1,1,0,1]
            * [202,1,0,1,1]
            * [203,1,2,3,4]
        */
        int attackNumber = array2D.Length - 1;
        int DefendNumber = array2D[0].Length - 1;
        //遍历整个表
        for (int i = 1; i <= attackNumber; i++)
        {
            for (int j = 1; j <= DefendNumber; j++)
            {
                if (Enum.TryParse(array2D[i][j], out CounterMethod counterMethod))
                    dict.Add((array2D[i][0], array2D[0][j]), counterMethod);
                else
                    Debug.Assert(false, "Wrong CounterMethod Input");
            }
        }
        return dict;
    }
}