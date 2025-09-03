using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EmotionDataBase
{
    public string path;
    public Dictionary<int, EmotionDefine> EmotionDictionary { get; set; }
    //读入所有玩家
    public void LoadingEmotion()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/Emotion.json");
        EmotionDictionary = JsonLoader.DeserializeObject<Dictionary<int, EmotionDefine>>(path);
        //打印行动类到日志
        Log.PrintLoadedDictionary(EmotionDictionary, "Log/Loading/AILog.txt");
    }
}
