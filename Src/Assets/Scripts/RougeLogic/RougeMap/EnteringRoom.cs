using UnityEngine;

public enum RoomRayStatus
{
    Disable = 0,
    ChooseRoom = 1,
    OnRoom = 2,
}

public class EnteringRoom : MonoBehaviour
{
    [SerializeField] private RoomRayStatus rayStatus = RoomRayStatus.ChooseRoom;
    private GameObject lastHoveredRoom;

    private void Update()
    {
        MouseAndRayUtil.Hit("Room", out var room);

        switch (rayStatus)
        {
            case RoomRayStatus.Disable:
                break;
            case RoomRayStatus.ChooseRoom:
                MouseAndRayUtil.RenewHitting(ref lastHoveredRoom, room);
                if (room != null)
                {
                    rayStatus = RoomRayStatus.OnRoom;
                }
                break;
            case RoomRayStatus.OnRoom:
                MouseAndRayUtil.RenewHitting(ref lastHoveredRoom, room);
                if (room == null)
                {
                    rayStatus = RoomRayStatus.ChooseRoom;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    room.GetComponent<RoomSelection>()?.EnterRoom();
                }
                break;
        }
    }

    public void Disable()
    {
        rayStatus = RoomRayStatus.Disable;
        if (lastHoveredRoom != null)
        {
            lastHoveredRoom.GetComponent<IHoverable>()?.OnHoverExit();
            lastHoveredRoom = null;
        }
    }

    public void Enable()
    {
        rayStatus = RoomRayStatus.ChooseRoom;
    }
}
