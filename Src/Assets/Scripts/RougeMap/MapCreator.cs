using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

//产生Map
public class MapCreator: MonoBehaviour
{
    //种子
    public int seed;
    //x方向单位和y方向单位
    public float x_unit;
    public float y_unit;
    //绘制地图需要的材料
    public GameObject RoomPrefab;
    public GameObject MapRoot;
    public GameObject lineRenderer;
    //房间表
    public List<List<Room>> RoomsbyFloor;
    //诅咒房间的可能性
    private int CursedPossibility = 0;
    //x方向随机波动和y方向随机波动
    //生成地图
    public void Start()
    {
        Random.InitState(seed);
        GenerateMap();
    }
    public void GenerateMap()
    {
        CursedPossibility = 0;
        RoomsbyFloor = new();
        //生成起始房间
        Room firstRoom = CreateRoom(0, 0, 0, RoomID.StartRoom);
        List<Room> firstRooms = new() { firstRoom };
        RoomsbyFloor.Add(firstRooms);
        //生成发育路线
        GenerateGrowthRoutes(17, firstRooms, out var finalRooms, new(0, 0));
        //赋值发育路线
        for (int i = 1; i <= 17; i++)
        {
            foreach (var room in RoomsbyFloor[i])
            {
                switch (i)
                {
                    //第一、三层小怪
                    case 1:
                    case 3:
                    case 7:
                    case 9:
                    case 13:
                    case 15:
                        AssignRoom(room, "Minion", CurseAvailable: true);break;
                    //第五层，进Boss或精英前小怪
                    case 5:
                    case 11:
                    case 17:
                        AssignRoom(room,"Minion",CurseAvailable: false);break;
                    //第二层发育节点
                    case 2:
                    case 8:
                    case 14:
                        AssignRoom(room, "LowClassRoom");break;
                    //第四层发育节点
                    case 4:
                    case 10:
                    case 16:
                        AssignRoom(room, "HighClassRoom"); break;
                    //精英
                    case 6:
                    case 12:
                        AssignRoom(room, "Elite"); break;
                    default:
                        Debug.Assert(false,"Not expected such a floor");
                        break;
                }

            }
        }
        //生产Boss房间
        Room BossRoom = CreateRoom(0, 18 * y_unit, 18, RoomID.Boss);
        foreach (var room in RoomsbyFloor[17])
        {
            room.NextNodes.Add(BossRoom);
        }
        RoomsbyFloor.Add(new List<Room>() { BossRoom });
        //画出地图
        DrawMap(RoomsbyFloor.SelectMany(x => x).ToList());
    }
    public void DrawMap(List<Room> rooms)
    {
        foreach(var room in rooms)
        {
            room.InitializeRoom();
            foreach(var nextRoom in room.NextNodes)
            {
                Instantiate(lineRenderer,MapRoot.transform);
                LineRenderer line = lineRenderer.GetComponent<LineRenderer>();
                line.SetPosition(0, MapRoot.transform.position + new Vector3(room.x, room.y));
                line.SetPosition(1, MapRoot.transform.position + new Vector3(nextRoom.x, nextRoom.y));
            }
        }
    }
    //AllRooms, starterRooms excluded
    public void GenerateGrowthRoutes(int floor,
        List<Room> StartRooms, out List<Room> finalRooms, Vector2 StartPosition)
    {
        List<Room> previousRooms = StartRooms;
        for (int i = 0; i < floor; i++)
        {
            List<Room> rooms = new();
            //生产随机2~5个节点
            int number = Random.Range(2, 6);
            for(int j = 0; j < number; j++)
            {
                //平移数
                float x_shift = ((j + 1) - (1 + number) / 2.0f) * x_unit;
                float y_shift = (i + 1) * y_unit;
                Room room = CreateRoom(StartPosition.x + x_shift, StartPosition.y + y_shift, i + 1, RoomID.Undecided);
                rooms.Add(room);
            }
            //处理节点问题//保证每一个Previous Room都能链接到现在的Room
            //链接第一步，先把每一个节点和其最相近的节点相连。选择多的那方去连
            ConnectRooms(previousRooms, rooms);
            previousRooms = new(rooms);
            //加入大数组
            RoomsbyFloor.Add(rooms);
        }
        finalRooms = previousRooms;
    }
    //维护这个函数和RoomID的数字大小关系
    private void AssignRoom(Room room, string Catagory, bool overRide = false, bool CurseAvailable = false)
    {
        if (!overRide && room.roomID != RoomID.Undecided)
            return;
        int number = 0;
        switch(Catagory)
        {
            case "Minion":
                number = Random.Range(0, 100);
                if (number < CursedPossibility && CurseAvailable)
                {
                    room.roomID = RoomID.DemonAlter;
                    foreach(Room nextRoom in room.NextNodes)
                    {
                        AssignRoom(nextRoom, "CursedRoom");
                    }
                    CursedPossibility -= 30;
                }
                else
                {
                    room.roomID = RoomID.Minion;
                    CursedPossibility += 3;
                }
                break;
            case "Elite":
                room.roomID = RoomID.Elite; break;
            case "Boss":
                room.roomID = RoomID.Boss; break;
            case "LowClassRoom":
                number = Random.Range(10, 17);
                room.roomID = (RoomID)(number); break;
            case "HighClassRoom":
                number = Random.Range(20, 22);
                room.roomID = (RoomID)(number); break;
            case "CursedRoom":
                number = Random.Range(40, 42);
                room.roomID = (RoomID)(number); break;
            default:
                Debug.Assert(false, $"未知的房间类别: {Catagory}");
                break;
        }
    }

    private Room CreateRoom(float x, float y, int floor, RoomID roomID)
    {
        GameObject roomObject = Instantiate(RoomPrefab, MapRoot.transform);
        Room room = roomObject.GetComponent<Room>();
        room.x = x;
        room.y = y;
        room.floor = floor;
        room.roomID = roomID;
        room.NextNodes = new();
        return room;
    }
    //连接房间
    private void ConnectRooms(List<Room> starterRooms, List<Room> targetRooms)
    {
        //标记每个room是否完成了
        List<Room> completedRooms = new();
        foreach(var room in starterRooms)
        {
            Room targetRoom = FindNearestRoom(room, targetRooms);
            completedRooms.Add(targetRoom);
            room.NextNodes.Add(targetRoom);
            Debug.Log("Add" + room.x + "," + room.y + "'s target" + targetRoom.x + "," + targetRoom.y);
        }
        foreach (var room in targetRooms)
        {
            //如果没有完成，补上
            if (!completedRooms.Contains(room))
            {
                Room reversedTargetRoom = FindNearestRoom(room, starterRooms);
                reversedTargetRoom.NextNodes.Add(room);
            }
        }
        /*        foreach(var room in targetRooms)
                {
                    Room targetRoom = FindNextNearestRoom(room, targetRooms, 0.1f);
                    if (targetRoom != null)
                    {
                        if (Random.Range(0, 10) > 7)
                            room.NextNodes.Add(targetRoom);
                    }
                }*/
    }
    
    //寻找最近的房间
    private Room FindNearestRoom(Room room, List<Room> targetRooms)
    {
        if (targetRooms.Count == 0 || targetRooms == null)
        {
            Debug.Assert(false, "TargetRooms can't be empty");
        }
        Room NearestRoom = null;
        float distance = 99;
        foreach(var targetRoom in targetRooms)
        {
            if (Mathf.Abs(room.x - targetRoom.x) < distance)
            {
                NearestRoom = targetRoom;
                distance = Mathf.Abs(room.x - targetRoom.x);
            }
        }
        return NearestRoom;
    }
    private Room FindNextNearestRoom(Room room, List<Room> targetRooms, float tolerence)
    {
        if (targetRooms.Count == 0 || targetRooms == null)
        {
            Debug.Assert(false, "TargetRooms can't be empty");
        }
        //必须是只链接一个的才给它找，TargetRooms里面必须不止一个
        if (room.NextNodes.Count != 1 || targetRooms.Count == 1)
            return null;
        List<Room> targetRoomsCopy = new(targetRooms);
        targetRoomsCopy.Remove(room.NextNodes[0]);
        Room targetRoom = FindNearestRoom(room, targetRoomsCopy);
        //先不实装tolerance
        return targetRoom;
    }
}
