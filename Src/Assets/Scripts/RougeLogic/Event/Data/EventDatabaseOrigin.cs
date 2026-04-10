using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EventDatabaseOrigin : MonoSingleton<EventDatabaseOrigin>
{
    public Dictionary<string, EventDefineOrigin> EventDictionary { get; private set; }

    /// <summary>
    /// Load RougeEvent.json into EventDictionary. Called by EventDatabase.Awake.
    /// </summary>
    public void LoadingEvents()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/RougeMap/RougeEvent.json");
        EventDictionary = JsonLoader.DeserializeObject<Dictionary<string, EventDefineOrigin>>(path);
        MyLog.PrintLoadedDictionary(EventDictionary, "Log/Loading/RougeEvents.txt");
    }
}
