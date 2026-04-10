using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



/// <summary>

/// Roguelike meta + run state (<see cref="rougePlayer"/>). Bonus-room offers run in dedicated scenes via

/// <see cref="OfferingUI"/> (SoulFountain / Tavern / SacredCemetery).

/// </summary>

public class RougeManager : MonoSingleton<RougeManager>
{
    public RougePlayer rougePlayer;
    public Room CurrentRoom { get; private set; }

    void Start()
    {
        rougePlayer = new();
        rougePlayer.InitializeWithTwoWukongHeroes();
        RougePlayerUI.Instance?.Initialize();
    }

    /// <summary>
    /// Returns true if target can be selected from current room:
    /// target must be on next floor and linked by CurrentRoom.NextNodes.
    /// </summary>
    public bool CanSelectRoom(Room targetRoom)
    {
        return CurrentRoom.NextNodes != null && CurrentRoom.NextNodes.Contains(targetRoom);
    }

    public void SetCurrentRoom(Room room)
    {
        if (room == null)
            return;
        CurrentRoom = room;
    }
}

