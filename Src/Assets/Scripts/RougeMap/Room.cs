using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//房间的定义
public class Room : MonoBehaviour
{
    //节点的坐标
    public float x;
    public float y;
    public int floor;
    public RoomID roomID;
    public List<Room> NextNodes;
    public void InitializeRoom()
    {
        transform.localPosition = new Vector3(x, y);
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        RoomLiberary.Instance.RoomDictionary.TryGetValue(roomID, out var roomTemplete);
        if(roomTemplete == null )
        {
            Debug.Assert(false, "Can't find RoomID" + roomID);
        }
        sprite.sprite = roomTemplete.image;
    }
    
}


