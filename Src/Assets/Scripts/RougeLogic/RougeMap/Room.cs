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
    public RoomID roomID { get; private set; }
    public List<Room> NextNodes;
    [Header("Selectable Pulse")]
    [SerializeField] private float selectablePulseAmplitude = 0.4f;
    [SerializeField] private float selectablePulseSpeed = 3.5f;

    private bool isSelectableVisual;
    private Vector3 baseScale;
    private float selectablePulsePhase;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        var currentRoom = RougeManager.Instance?.CurrentRoom;
        if (currentRoom?.NextNodes != null && currentRoom.NextNodes.Contains(this))
        {
            float pulse = 1f + Mathf.Sin(Time.time * selectablePulseSpeed + selectablePulsePhase) * selectablePulseAmplitude;
            transform.localScale = baseScale * pulse;
        }
        else
            transform.localScale = baseScale;
    }
    public void InitializeRoom()
    {
        // Each room gets a different sine phase so selectable nodes don't pulse in sync.
        selectablePulsePhase = (x * 2.173f + y * 3.419f + floor * 0.812f);

        transform.localPosition = new Vector3(x, y);
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        RoomLiberary.Instance.RoomDictionary.TryGetValue(roomID, out var roomTemplete);
        if(roomTemplete == null)
        {
            Debug.Assert(false, "Can't find RoomID" + roomID);
        }
        sprite.sprite = roomTemplete.image;
    }
    //被赋值一次，则概率变为原本的1/5
    public void AssignRoom(RoomID roomID)
    {
        this.roomID = roomID;
        if (MapDesigner.Instance.RoomProbabilityDic.ContainsKey(roomID))
        {
            MapDesigner.Instance.RoomProbabilityDic[roomID] /= 5;
        }
    }

    /// <summary>
    /// Toggle selectable visual feedback. When true, room continuously scales up/down.
    /// </summary>
    public void SetSelectableVisual(bool selectable)
    {
        isSelectableVisual = selectable;
        if (!isSelectableVisual)
            transform.localScale = baseScale;
    }
}


