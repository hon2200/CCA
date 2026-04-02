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
    public int CursedPossibility = 0;

    [Header("Temporary room stand-ins (not final content)")]
    [Tooltip("When true: Alchemy/Tailor assign as Minion; curse branch uses Treasure instead of EvilForge. Set false when those rooms are implemented.")]
    public bool useTemporaryRoomStandins = false;

    private RoomID CurseBranchForgeOrTreasure()
    {
        return useTemporaryRoomStandins ? RoomID.Treasure : RoomID.EvilForge;
    }

    // Shop-type bonus rooms (markets, workshops, bank, tavern). Only floors i=11 and i=17 use these; other Bonus floors exclude them.
    private static bool IsShopRoom(RoomID id)
    {
        switch (id)
        {
            case RoomID.TalentMarket:
            case RoomID.AntiqueMarket:
            case RoomID.CardMarket:
                return true;
            default:
                return false;
        }
    }

    private Dictionary<RoomID, float> BuildProbabilityDic(bool shopsOnly)
    {
        var d = new Dictionary<RoomID, float>();
        foreach (var kvp in RoomProbabilityDic)
        {
            if (IsShopRoom(kvp.Key) == shopsOnly)
                d[kvp.Key] = kvp.Value;
        }
        return d;
    }

    public void Awake()
    {
        InitializeRoomProbabilityDic();
    }

    // Initialize per-room-type weights used when drawing Bonus / Shop assignments.
    // Scan the numeric bonus band; only values that exist on RoomID are added (e.g. 16 is skipped).
    public void InitializeRoomProbabilityDic()
    {
        RoomProbabilityDic = new();
        const int bonusPoolIdMin = 10;
        const int bonusPoolIdMax = 19;
        for (int v = bonusPoolIdMin; v <= bonusPoolIdMax; v++)
        {
            if (!Enum.IsDefined(typeof(RoomID), v))
                continue;
            RoomProbabilityDic[(RoomID)v] = 1f;
        }
    }

    // Assign room types for every floor except the start room (index 0).
    public void AssignAllRooms(List<List<Room>> roomByFloors)
    {
        for (int i = 1; i < roomByFloors.Count; i++)
        {
            if (i == 11 || i == 17)
            {
                AssignRooms(roomByFloors[i], "Shop");
                continue;
            }
            // Minion floors (odd index; curse rules differ when i % 6 == 5).
            if (i % 2 == 1)
                if (i % 6 != 5)
                    AssignRooms(roomByFloors[i], "Minion", CurseAvailable: true);
                else
                    AssignRooms(roomByFloors[i], "Minion", CurseAvailable: false);
            // Elite or Boss on every 6th floor; floor 18 is Boss.
            else if (i % 6 == 0)
                if (i == 18)
                    AssignRooms(roomByFloors[i], "Boss");
                else
                    AssignRooms(roomByFloors[i], "Elite");
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
        // One draw list for all Bonus or Shop slots on this floor.
        List<RoomID> BonusRoomsList = new();
        foreach (var room in rooms)
        {
            if (room.roomID != RoomID.Undecided && overRide == false)
                continue;
            switch (Catagory)
            {
                case "Minion":
                    number = UnityEngine.Random.Range(0, 100);
                    if (number < CursedPossibility && CurseAvailable)
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
                            room.AssignRoom(RoomID.DemonAlter);
                            foreach (Room nextRoom in room.NextNodes)
                            {
                                number = UnityEngine.Random.Range(0, 100);
                                if (number > 50)
                                    nextRoom.AssignRoom(RoomID.CurseFusion);
                                else
                                    nextRoom.AssignRoom(CurseBranchForgeOrTreasure());
                            }
                            CursedPossibility -= 20;
                        }
                        else
                        {
                            room.AssignRoom(RoomID.Minion);
                            CursedPossibility += 2;
                        }
                    }
                    else
                    {
                        room.AssignRoom(RoomID.Minion);
                        CursedPossibility += 2;
                    }
                    break;
                case "Elite":
                    room.AssignRoom(RoomID.Elite); break;
                case "Boss":
                    room.AssignRoom(RoomID.Boss); break;
                case "Bonus":
                    // Non-shop bonus pool; shops only on floors i=11 and i=17 (Shop category).
                    if (BonusRoomsList.Count == 0)
                    {
                        var bonusNoShops = BuildProbabilityDic(shopsOnly: false);
                        BonusRoomsList = SelectByFloatProbability<RoomID>(bonusNoShops, rooms.Count);
                    }
                    room.AssignRoom(BonusRoomsList[count]);
                    count++;
                    break;
                case "Shop":
                    if (BonusRoomsList.Count == 0)
                    {
                        var shopDic = BuildProbabilityDic(shopsOnly: true);
                        BonusRoomsList = SelectByFloatProbability<RoomID>(shopDic, rooms.Count);
                    }
                    room.AssignRoom(BonusRoomsList[count]);
                    count++;
                    break;
                default:
                    Debug.Assert(false, $"Unknown room category: {Catagory}");
                    break;
            }
        }

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
