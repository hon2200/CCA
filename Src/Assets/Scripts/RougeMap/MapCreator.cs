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
    //房间总数（起始房除外）
    private int totalFloor = 18;
    //房间表
    public List<List<Room>> RoomsbyFloor;
    //额外分支生产概率
    private int additionalRouteProb = 10;
    //x方向随机波动和y方向随机波动
    public float x_float;
    public float y_float;
    //生成地图
    public void Start()
    {
        Random.InitState(seed);
        GenerateMap();
    }
    public void GenerateMap()
    {
        RoomsbyFloor = new();
        //生成起始房间
        Room firstRoom = CreateRoom(0, 0, 0, RoomID.StartRoom);
        List<Room> firstRooms = new() { firstRoom };
        RoomsbyFloor.Add(firstRooms);
        //生成发育路线
        GenerateGrowthRoutes(firstRooms, out var finalRooms, new(0, 0));
        //分配房间
        MapDesigner.Instance.AssignAllRooms(RoomsbyFloor);
        //浮动房间
        FloatingRooms();
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
    public void GenerateGrowthRoutes(List<Room> StartRooms, out List<Room> finalRooms, Vector2 StartPosition)
    {
        List<Room> previousRooms = StartRooms;
        int previousNumber = 1;
        for (int i = 1; i <= totalFloor; i++)
        {
            List<Room> rooms = new();
            //开始处理房间数
            int number = 0;
            //精英和Boss只有一个节点(i=6,i=12,i=18)
            //精英和Boss前后必须有选择，不能只是一个节点。
            //其他地方的节点数为原先节点加减1
            if (i % 6 == 0)
            {
                number = 1;
            }
            //出精英后可能两个可能三个节点
            else if (i % 6 == 1)
            {
                number = Random.Range(2, 4);
            }
            //进精英Boss前锁定两个节点
            else if (i % 6 == 5)
            {
                number = 2;
            }
            else
            {
                //可能正负1
                int numberShift = Random.Range(-1, 2);
                //最大三个，最小一个
                number = Mathf.Clamp(previousNumber + numberShift, 2, 4);
            }
            previousNumber = number;
            //开始生成房间
            for(int j = 0; j < number; j++)
            {
                //平移数
                float x_shift = ((j + 1) - (1 + number) / 2.0f) * x_unit;
                float y_shift = i * y_unit;
                Room room = CreateRoom(StartPosition.x + x_shift, StartPosition.y + y_shift, i, RoomID.Undecided);
                rooms.Add(room);
            }
            //处理节点问题//保证每一个Previous Room都能链接到现在的Room
            //链接第一步，先把每一个节点和其最相近的节点相连。选择多的那方去连
            ConnectRooms(previousRooms, rooms);
            previousRooms = new(rooms);
            //加入大数组
            RoomsbyFloor.Add(rooms);
            Debug.Log("第" + i + "层" + rooms.Count + "个");
        }
        finalRooms = previousRooms;
    }
    private Room CreateRoom(float x, float y, int floor, RoomID roomID)
    {
        GameObject roomObject = Instantiate(RoomPrefab, MapRoot.transform);
        Room room = roomObject.GetComponent<Room>();
        room.x = x;
        room.y = y;
        room.floor = floor;
        room.AssignRoom(roomID);
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
        foreach(var room in starterRooms)
        {
            int number = Random.Range(0, 100);
            if (number < additionalRouteProb)
            {
                Room additionalRoom = FindNextNearestRoom(room, starterRooms, targetRooms);
                if (additionalRoom != null)
                {
                    room.NextNodes.Add(additionalRoom);
                    additionalRouteProb -= 20;
                    break;
                }
            }
            else
            {
                additionalRouteProb += 4;
            }
        }
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
    private Room FindNextNearestRoom(Room room, List<Room> startRooms, List<Room> targetRooms)
    {
        if (targetRooms.Count == 0 || targetRooms == null)
        {
            Debug.Assert(false, "TargetRooms can't be empty");
        }
        //必须是只链接一个的才给它找，TargetRooms里面必须不止一个
        if (room.NextNodes.Count != 1 || targetRooms.Count == 1)
            return null;
        if (!MapDesigner.Instance.IsOnlyTarget(room.NextNodes[0], startRooms))
            return null;
        List<Room> targetRoomsCopy = new(targetRooms);
        targetRoomsCopy.Remove(room.NextNodes[0]);
        Room targetRoom = FindNearestRoom(room, targetRoomsCopy);
        if (!MapDesigner.Instance.IsOnlyTarget(targetRoom, startRooms))
            return null;
        return targetRoom;
    }
    private void FloatingRooms()
    {
        foreach(var rooms in RoomsbyFloor)
        {
            if(rooms.Count == 1) 
                continue;
            foreach(var room in rooms)
            {
                float x_shift = Random.Range(-1.0f, 1.0f) * x_float;
                float y_shift = Random.Range(-1.0f, 1.0f) * y_float;
                room.x += x_shift;
                room.y += y_shift;
            }
        }

    }
}
