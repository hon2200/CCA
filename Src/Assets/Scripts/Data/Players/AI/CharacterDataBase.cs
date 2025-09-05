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
    public Dictionary<string, CharacterDefine> CharacterDictionary { get; set; }
    //读入所有玩家
    public void LoadingCharacter()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/Character.json");
        CharacterDictionary = JsonLoader.DeserializeObject<Dictionary<string, CharacterDefine>>(path);
        //打印行动类到日志
        Log.PrintLoadedDictionary(CharacterDictionary, "Log/Loading/CharacterLog.txt");
    }
}
