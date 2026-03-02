using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class RelicDatabaseOrigin : MonoSingleton<RelicDatabaseOrigin>
{
    public Dictionary<string, RelicDefineOrigin> RelicDictionary { get; private set; }

    /// <summary>
    /// Load Relic.json into RelicDictionary. Called by RelicDatabase.Awake.
    /// </summary>
    public void LoadingRelics()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Relic/Relic.json");
        RelicDictionary = JsonLoader.DeserializeObject<Dictionary<string, RelicDefineOrigin>>(path);
        MyLog.PrintLoadedDictionary(RelicDictionary, "Log/Loading/Relics.txt");
    }
}