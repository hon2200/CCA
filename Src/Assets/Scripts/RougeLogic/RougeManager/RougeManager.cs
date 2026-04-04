using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



/// <summary>

/// Roguelike meta + run state (<see cref="rougePlayer"/>). Bonus-room offers run in dedicated scenes via

/// <see cref="OfferingUI"/> (SoulFountain / Tavern / SacredCemetery).

/// </summary>

public partial class RougeManager : MonoSingleton<RougeManager>
{
    public RougePlayer rougePlayer;
    void Start()
    {
        rougePlayer = new();
        rougePlayer.InitializeWithTwoWukongHeroes();
        RougePlayerUI.Instance.Initialize();
    }
}

