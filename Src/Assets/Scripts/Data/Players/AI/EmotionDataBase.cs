using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EmotionDataBase : MonoSingleton<EmotionDataBase>
{
    public string path;
    public Dictionary<EmotionType, EmotionDefine> EmotionDictionary { get; set; }
    public void Start()
    {
        LoadingEmotion();
    }
    //读入所有玩家
    public void LoadingEmotion()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/Emotion.json");
        EmotionDictionary = JsonLoader.DeserializeObject<Dictionary<EmotionType, EmotionDefine>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(EmotionDictionary, "Log/Loading/EmotionLog.txt");
    }
}
