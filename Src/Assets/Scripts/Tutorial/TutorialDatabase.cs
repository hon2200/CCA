using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialDatabase : MonoSingleton<TutorialDatabase>
{
    public string path;
    public Dictionary<string, TutorialDefine> TutorialDictionary { get; set; }

    private void Awake()
    {
        LoadingTutorial();
    }

    public void LoadingTutorial()
    {
        path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero&Enemy/Tutorial.json");
        TutorialDictionary = JsonLoader.DeserializeObject<Dictionary<string, TutorialDefine>>(path);
        MyLog.PrintLoadedDictionary(TutorialDictionary, "Log/Loading/TutorialLog.txt");
    }
}
