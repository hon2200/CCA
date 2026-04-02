using UnityEngine;

public class RoomSelection : HoverableBase
{
    [SerializeField] private GameObject glow;
    public Room room;
    public void Awake()
    {
        room = GetComponent<Room>();
    }
    public override void OnHoverEnter(Vector3? scaleMultiplier, Quaternion? rotationOffset, Vector3? positionOffset,
        Quaternion? rotationFinal, Vector3? positionFinal)
    {
        base.OnHoverEnter(scaleMultiplier, rotationOffset, positionOffset, rotationFinal, positionFinal);
        if (glow != null)
        {
            glow.SetActive(true);
        }
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
        if (glow != null)
        {
            glow.SetActive(false);
        }
    }

    public void EnterRoom()
    {
        Debug.Log($"EnterRoom: {gameObject.name}");
        switch (room.roomID)
        {
            case RoomID.Alchemyworkshop:
            case RoomID.TailorShop:
            case RoomID.EvilForge:
            case RoomID.Treasure:
                Debug.Log("Get Gold");
                break;
            case RoomID.AntiqueMarket:
                Debug.Log("Enter AntiqueMarket");
                break;
            case RoomID.TalentMarket:
                Debug.Log("Enter TalentMarket");
                break;
            case RoomID.CardMarket:
                Debug.Log("Enter CardMarket");
                break;
            case RoomID.CurseFusion:
                Debug.Log("Enter CurseFusion");
                break;
            case RoomID.DemonAlter:
                Debug.Log("Enter DemonAlter");
                break;
            case RoomID.Tavern:
                Debug.Log("Enter Tavern");
                break;
            case RoomID.SacredCemetery:
                Debug.Log("Enter SacredCemetery");
                break;
            case RoomID.Minion:
            case RoomID.Elite:
            case RoomID.Boss:
                Debug.Log("Enter Fight");
                break;
        }

    }
}
