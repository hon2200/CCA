using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterDataBase : MonoSingleton<CharacterDataBase>
{
    public string path;
    public Dictionary<int, CharacterDefine> AIDictionary { get; set; }
    //读入所有玩家
    public void LoadingCharacter()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/Character.json");
        AIDictionary = JsonLoader.DeserializeObject<Dictionary<int, CharacterDefine>>(path);
        //打印行动类到日志
        Log.PrintLoadedDictionary(AIDictionary, "Log/Loading/CharacterLog.txt");
    }
}
