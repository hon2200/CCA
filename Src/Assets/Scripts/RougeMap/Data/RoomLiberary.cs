using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;

//房间图书馆，通过和ActionDataBase一样的Key键索引卡牌，通过CardTemplete获得预制体卡牌以及其实例所需要的资源
public class RoomLiberary : MonoSingleton<RoomLiberary>
{
    [Tooltip("Path under Resources folder (e.g. 'Rooms' for Assets/Resources/Rooms)")]
    [SerializeField] private string _resourcesPath = "Scriptables/RoomScriptables";

    //所有房间汇总
    public SerializedDictionary<RoomID, RoomTemplete> RoomDictionary { get; private set; }

    protected override void OnStart()
    {
        LoadAllRooms();
    }

    /// <summary>
    /// Load all RoomTemplete ScriptableObjects from the Resources folder and fill RoomDictionary.
    /// </summary>
    public void LoadAllRooms()
    {
        RoomTemplete[] templates = Resources.LoadAll<RoomTemplete>(_resourcesPath);
        RoomDictionary = new SerializedDictionary<RoomID, RoomTemplete>();

        foreach (RoomTemplete template in templates)
        {
            if (template == null) continue;
            if (RoomDictionary.ContainsKey(template.ID))
            {
                Debug.LogWarning($"RoomLiberary: Duplicate RoomID '{template.ID}' for asset '{template.name}', skipping.");
                continue;
            }
            RoomDictionary.Add(template.ID, template);
        }

        Debug.Log($"RoomLiberary: Loaded {RoomDictionary.Count} room(s) from Resources/{_resourcesPath}.");
    }
}