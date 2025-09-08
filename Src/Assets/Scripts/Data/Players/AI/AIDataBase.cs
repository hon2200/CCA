using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//这里用来装关于AI的定义
//Json文件->AIDataBase->每个Player的PlayerDefine中
public class AIDataBase : MonoSingleton<AIDataBase>
{
    public string path;
    public Dictionary<string, AIDefine> AIDictionary { get; set; }
    public void Start()
    {
        LoadingAI();
    }
    //读入所有玩家
    public void LoadingAI()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/AI.json");
        AIDictionary = JsonLoader.DeserializeObject<Dictionary<string, AIDefine>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(AIDictionary,"Log/Loading/AILog.txt");
    }
}
