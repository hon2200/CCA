using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;

public class MapDesigner : MonoSingleton<MapDesigner>
{
    public Dictionary<RoomID, float> RoomProbabilityDic;
    [SerializeField] private EliteReplacementController eliteReplacement = new();

    [Header("Temporary room stand-ins (not final content)")]
    [Tooltip("When true: Alchemy/Tailor assign as Minion; curse branch uses Treasure instead of EvilForge. Set false when those rooms are implemented.")]
    public bool useTemporaryRoomStandins = false;

    private RoomID CurseBranchForgeOrTreasure()
    {
        return useTemporaryRoomStandins ? RoomID.Treasure : RoomID.EvilForge;
    }

    public void Awake()
    {
        InitializeRoomProbabilityDic();
    }

    // Initialize per-room-type weights used when drawing Bonus / Shop assignments.
    // Only use the configured bonus-event rooms for random bonus assignment.
    public void InitializeRoomProbabilityDic()
    {
        RoomProbabilityDic = new();
        RoomProbabilityDic[RoomID.Tavern] = 1f;          // 11
        RoomProbabilityDic[RoomID.SacredCemetery] = 1f;  // 12
        RoomProbabilityDic[RoomID.SoulFountain] = 1f;    // 13
        RoomProbabilityDic[RoomID.Shelter] = 1f;         // 14
        RoomProbabilityDic[RoomID.CurseFusion] = 1f;     // 17
        RoomProbabilityDic[RoomID.Treasure] = 1f;        // 18
    }

    // Assign room types for every floor except the start room (index 0).
    public void AssignAllRooms(List<List<Room>> roomByFloors)
    {
        eliteReplacement.ResetRunState();

        for (int i = 1; i < roomByFloors.Count; i++)
        {
            // Boss floor.
            if (i == 18)
                AssignRooms(roomByFloors[i], "Boss");
            // Minion floors.
            else if (i % 2 == 1)
                AssignRooms(roomByFloors[i], "Minion", CurseAvailable: i >= 5);
            // Bonus / growth floors (even, not divisible by 6).
            else
                AssignRooms(roomByFloors[i], "Bonus");
        }
    }

    // Keep Category strings and RoomID numeric ranges in sync when editing room tables.
    private void AssignRooms(List<Room> rooms, string Catagory, bool overRide = false, bool CurseAvailable = false)
    {
        // Index into the precomputed Bonus/Shop pick list for this floor.
        int count = 0;
        int number = 0;
        bool previousRoomIsElite = false;
        // One draw list for all Bonus or Shop slots on this floor.
        List<RoomID> BonusRoomsList = new();
        foreach (var room in rooms)
        {
            if (room.roomID != RoomID.Undecided && overRide == false)
            {
                previousRoomIsElite = room.roomID == RoomID.Elite;
                continue;
            }
            switch (Catagory)
            {
                case "Minion":
                    // Restrict adjacent elites on the same floor.
                    bool canTryElite = CurseAvailable && !previousRoomIsElite;
                    if (canTryElite && eliteReplacement.ShouldTryEliteReplacement(out _, out _))
                    {
                        // Curse layout only if every linked next room is uniquely reachable from this floor.
                        bool onlyNextRoom = true;
                        foreach (var nextRoom in room.NextNodes)
                        {
                            if (!IsOnlyTarget(nextRoom, rooms))
                                onlyNextRoom = false;
                        }
                        if (onlyNextRoom)
                        {
                            room.AssignRoom(RoomID.Elite);
                            foreach (Room nextRoom in room.NextNodes)
                            {
                                if (nextRoom.floor == 18)
                                    continue;
                                // Raise the possibility of Random Rooms
                            }
                            eliteReplacement.RecordResult(success: true);
                        }
                        else
                        {
                            // Topology blocked: keep Minion and do not change pressure.
                            room.AssignRoom(RoomID.Minion);
                        }
                    }
                    else
                    {
                        room.AssignRoom(RoomID.Minion);
                        if (CurseAvailable)
                            eliteReplacement.RecordResult(success: false);
                    }
                    break;
                case "Boss":
                    room.AssignRoom(RoomID.Boss); break;
                case "Bonus":
                    if (BonusRoomsList.Count == 0)
                    {
                        BonusRoomsList = SelectByFloatProbability<RoomID>(RoomProbabilityDic, rooms.Count);
                    }
                    room.AssignRoom(BonusRoomsList[count]);
                    count++;
                    break;
                default:
                    Debug.Assert(false, $"Unknown room category: {Catagory}");
                    break;
            }
            previousRoomIsElite = room.roomID == RoomID.Elite;
        }

    }

    private RoomID GetRandomBonusRoom()
    {
        var picks = SelectByFloatProbability<RoomID>(RoomProbabilityDic, 1);
        if (picks.Count == 0)
            return RoomID.Minion;
        return picks[0];
    }

    // Weighted random sample without replacement (m picks from the dictionary keys).
    private List<T> SelectByFloatProbability<T>(Dictionary<T, float> probabilityDict, int m)
    {
        if (probabilityDict == null || probabilityDict.Count == 0)
        {
            Debug.LogError("Probability dictionary cannot be empty.");
            return new List<T>();
        }

        if (m > probabilityDict.Count)
        {
            Debug.LogWarning($"Requested {m} distinct picks but the dictionary only has {probabilityDict.Count} entries.");
            return probabilityDict.Keys.ToList();
        }

        float totalProbability = probabilityDict.Values.Sum();

        if (Mathf.Abs(totalProbability - 1f) > 0.01f)
        {
            Debug.LogWarning($"Total weight is {totalProbability} (not 1); selection still uses raw weights.");
        }

        Dictionary<T, float> tempDict = new Dictionary<T, float>(probabilityDict);
        List<T> result = new List<T>();

        for (int i = 0; i < m; i++)
        {
            float currentTotal = tempDict.Values.Sum();

            float randomValue = UnityEngine.Random.Range(0f, currentTotal);

            float cumulative = 0f;
            T selected = default(T);

            foreach (var kvp in tempDict)
            {
                cumulative += kvp.Value;

                if (randomValue < cumulative || Mathf.Approximately(randomValue, cumulative))
                {
                    selected = kvp.Key;
                    break;
                }
            }

            if (selected == null || selected.Equals(default(T)))
            {
                selected = tempDict.Keys.First();
            }

            result.Add(selected);
            tempDict.Remove(selected);
        }

        return result;
    }

    // True if exactly one room on this floor links to targetRoom (unique successor from this floor).
    public bool IsOnlyTarget(Room targetRoom, List<Room> startRooms)
    {
        int connectedNumber = 0;
        foreach (var otherRoom in startRooms)
        {
            if (otherRoom.NextNodes.Contains(targetRoom))
            {
                connectedNumber++;
            }
        }
        return connectedNumber == 1;
    }
}
