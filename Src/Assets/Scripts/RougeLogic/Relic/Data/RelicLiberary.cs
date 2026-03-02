using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class RelicLiberary : MonoSingleton<RelicLiberary>
{
    [SerializeField]
    public SerializedDictionary<string, RelicTemplete> RelicDictionary;
}

