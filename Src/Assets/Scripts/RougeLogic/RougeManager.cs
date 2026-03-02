using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeManager : MonoSingleton<RougeManager>
{
    public RougePlayer rougePlayer;
    public void Awake()
    {
        rougePlayer = new();
    }
}