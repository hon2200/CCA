using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuffDataBase : MonoSingleton<BuffDataBase>
{
    public string path;
    public Dictionary<string, BuffDefine> BuffDictionary { get; set; }
    //读入所有玩家
    public void LoadingSkills()
    {
        path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero&Enemy/Buff.json");
        BuffDictionary = JsonLoader.DeserializeObject<Dictionary<string, BuffDefine>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(BuffDictionary, "Log/Loading/Buff.txt");
    }
}
