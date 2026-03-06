using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;

//卡牌图书馆，通过和ActionDataBase一样的Key键索引卡牌，通过CardTemplete获得预制体卡牌以及其实例所需要的资源
public class CardLiberary : MonoSingleton<CardLiberary>
{
    [Tooltip("Path under Resources folder (e.g. 'Scriptables/CardScriptables' for Assets/Resources/Scriptables/CardScriptables)")]
    [SerializeField] private string _resourcesPath = "Scriptables/CardScriptables";

    public SerializedDictionary<string, CardTemplete> CardDictionary { get; private set; }

    protected override void OnStart()
    {
        LoadAllCards();
    }

    /// <summary>
    /// Load all CardTemplete ScriptableObjects from the Resources folder and fill CardDictionary.
    /// </summary>
    public void LoadAllCards()
    {
        CardTemplete[] templates = Resources.LoadAll<CardTemplete>(_resourcesPath);
        CardDictionary = new SerializedDictionary<string, CardTemplete>();

        foreach (CardTemplete template in templates)
        {
            if (template == null) continue;
            if (string.IsNullOrEmpty(template.ID))
            {
                Debug.LogWarning($"CardLiberary: CardTemplete '{template.name}' has empty ID, skipping.");
                continue;
            }
            if (CardDictionary.ContainsKey(template.ID))
            {
                Debug.LogWarning($"CardLiberary: Duplicate ID '{template.ID}' for asset '{template.name}', skipping.");
                continue;
            }
            CardDictionary.Add(template.ID, template);
        }

        Debug.Log($"CardLiberary: Loaded {CardDictionary.Count} card(s) from Resources/{_resourcesPath}.");
    }
}
