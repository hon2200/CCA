using UnityEngine;
using UnityEngine.SceneManagement;

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
        switch (room.roomID)
        {
            case RoomID.Tavern:
                Debug.Log("Enter Tavern");
                RougeManager.SetPendingRoom(RoomID.Tavern);
                if (WorkingOn.Instance != null)
                    WorkingOn.Instance.LoadScene("Free Game");
                else
                    SceneManager.LoadScene("Free Game");
                break;
            case RoomID.SoulFountain:
                Debug.Log("Enter SoulFountain");
                RougeManager.SetPendingRoom(RoomID.SoulFountain);
                if (WorkingOn.Instance != null)
                    WorkingOn.Instance.LoadScene("Free Game");
                else
                    SceneManager.LoadScene("Free Game");
                break;
            case RoomID.SacredCemetery:
                Debug.Log("Enter SacredCemetery");
                RougeManager.SetPendingRoom(RoomID.SacredCemetery);
                if (WorkingOn.Instance != null)
                    WorkingOn.Instance.LoadScene("Free Game");
                else
                    SceneManager.LoadScene("Free Game");
                break;
            case RoomID.Minion:
            case RoomID.Elite:
            case RoomID.Boss:
                string fightType = room.roomID.ToString();
                var db = RougeFightsDatabase.Instance;
                if (db == null)
                {
                    Debug.LogError("[RoomSelection] RougeFightsDatabase.Instance is null.");
                    return;
                }

                var pickedFight = db.PickRandomFightByType(fightType);
                if (pickedFight == null)
                {
                    Debug.LogError($"[RoomSelection] No fight found for type: {fightType}");
                    return;
                }
                RougeManager.SetPendingFight(pickedFight.ID);
                RougeManager.SetPendingRoom(room.roomID);
                Debug.Log($"Enter Fight queued: type={fightType}, id={pickedFight.ID}. Loading Free Game.");
                if (WorkingOn.Instance != null)
                    WorkingOn.Instance.LoadScene("Free Game");
                else
                    SceneManager.LoadScene("Free Game");
                break;
        }

    }
}
