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
    public void Awake()
    {
        InitializeRoomProbabilityDic();
    }
    //初始化房间可能性
    public void InitializeRoomProbabilityDic()
    {
        RoomProbabilityDic = new();
        for (int i = 10; i <= 17; i++)
        {
            RoomProbabilityDic.Add((RoomID)i, 1);
        }
    }
    //分配除起始房之外的所有房间
    public void AssignAllRooms(List<List<Room>> roomByFloors)
    {
        for (int i = 1; i < roomByFloors.Count; i++)
        {
            //小怪房
            if (i % 2 == 1)
                if (i % 6 != 5)
                    AssignRooms(roomByFloors[i], "Minion", CurseAvailable: true);
                else
                    AssignRooms(roomByFloors[i], "Minion", CurseAvailable: false);
            //精英Boss房
            else if (i % 6 == 0)
                if (i == 18)
                    AssignRooms(roomByFloors[i], "Boss");
                else
                    AssignRooms(roomByFloors[i], "Elite");
            //发育房
            else
                AssignRooms(roomByFloors[i], "Bonus");
        }
    }

    //维护这个函数和RoomID的数字大小关系
    private void AssignRooms(List<Room> rooms, string Catagory, bool overRide = false, bool CurseAvailable = false)
    {
        //选择到第几个BonusRooms了
        int count = 0;
        //随机数
        int number = 0;
        //BonusRoom的可选项
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
                        //所有下一个房间都需要是被唯一链接的
                        bool onlyNextRoom = true;
                        foreach(var nextRoom in room.NextNodes)
                        {
                            if (!IsOnlyTarget(nextRoom, rooms))
                                onlyNextRoom = false;
                        }
                        //是，则变化
                        if (onlyNextRoom)
                        {
                            room.AssignRoom(RoomID.DemonAlter);
                            foreach (Room nextRoom in room.NextNodes)
                            {
                                number = UnityEngine.Random.Range(0, 100);
                                if (number > 50)
                                    nextRoom.AssignRoom(RoomID.CurseFusion);
                                else
                                    nextRoom.AssignRoom(RoomID.EvilForge);
                            }
                            CursedPossibility -= 20;
                        }
                        //否，则不变
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
                    //选择一下可用的BonusRooms
                    if(BonusRoomsList.Count == 0)
                    {
                        BonusRoomsList = SelectByFloatProbability<RoomID>(RoomProbabilityDic, rooms.Count);
                    }
                    room.AssignRoom(BonusRoomsList[count]);
                    count++;
                    break;
                default:
                    Debug.Assert(false, $"未知的房间类别: {Catagory}");
                    break;
            }
        }

    }
    //按概率选择的方法函数

    private List<T> SelectByFloatProbability<T>(Dictionary<T, float> probabilityDict, int m)
    {
        if (probabilityDict == null || probabilityDict.Count == 0)
        {
            Debug.LogError("概率字典不能为空");
            return new List<T>();
        }

        if (m > probabilityDict.Count)
        {
            Debug.LogWarning($"请求选择 {m} 个元素，但字典只有 {probabilityDict.Count} 个元素");
            return probabilityDict.Keys.ToList();
        }

        // 计算总概率（浮点数）
        float totalProbability = probabilityDict.Values.Sum();

        // 可选：检查总概率是否接近1（如果是归一化概率）
        if (Mathf.Abs(totalProbability - 1f) > 0.01f)
        {
            Debug.LogWarning($"总概率 {totalProbability} 不等于1，将进行归一化处理");
        }

        // 创建临时字典副本
        Dictionary<T, float> tempDict = new Dictionary<T, float>(probabilityDict);
        List<T> result = new List<T>();

        for (int i = 0; i < m; i++)
        {
            // 重新计算当前总概率
            float currentTotal = tempDict.Values.Sum();

            // 生成 [0, currentTotal) 范围内的随机浮点数
            float randomValue = UnityEngine.Random.Range(0f, currentTotal);

            // 轮盘赌选择
            float cumulative = 0f;
            T selected = default(T);

            foreach (var kvp in tempDict)
            {
                cumulative += kvp.Value;

                // 使用容差处理浮点数精度问题
                if (randomValue < cumulative || Mathf.Approximately(randomValue, cumulative))
                {
                    selected = kvp.Key;
                    break;
                }
            }

            // 安全检查：如果没有选中（可能由于浮点精度），选第一个
            if (selected == null || selected.Equals(default(T)))
            {
                selected = tempDict.Keys.First();
            }

            result.Add(selected);
            tempDict.Remove(selected);
        }

        return result;
    }
    //检查房间是否唯一
    public bool IsOnlyTarget(Room targetRoom, List<Room> startRooms)
    {
        //查看是不是下一个房间是不是唯一相连
        int connectedNumber = 0;
        foreach (var otherRoom in startRooms)
        {
            if (otherRoom.NextNodes.Contains(targetRoom))
            {
                connectedNumber++;
            }
        }
        //如果相连是1返回真
        return connectedNumber == 1;
    }
}