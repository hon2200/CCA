using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSelection : HoverableBase
{
    private const string FreeGameSceneName = "Free Game";

    private static RougeFightDefine _pendingRougeFight;

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
            glow.SetActive(true);
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
        if (glow != null)
            glow.SetActive(false);
    }

    public void EnterRoom()
    {
        if (room == null)
            return;

        switch (room.roomID)
        {
            case RoomID.Minion:
            case RoomID.Elite:
            case RoomID.Boss:
                EnterCombatRoom();
                break;
        }
    }

    private void EnterCombatRoom()
    {
        string fightType = room.roomID switch
        {
            RoomID.Minion => "Minion",
            RoomID.Elite => "Elite",
            RoomID.Boss => "Boss",
            _ => null
        };

        if (string.IsNullOrEmpty(fightType))
            return;

        if (RougeFightsDatabase.Instance == null)
        {
            Debug.LogError("[RoomSelection] RougeFightsDatabase.Instance is null.");
            return;
        }

        _pendingRougeFight = RougeFightsDatabase.Instance.PickRandomFightByType(fightType);
        if (_pendingRougeFight == null)
        {
            Debug.LogError($"[RoomSelection] No fight definition for type {fightType}.");
            return;
        }

        SceneManager.sceneLoaded += OnFreeGameLoadedForRougeFight;
        LoadRougeScene(FreeGameSceneName);
    }

    private static void OnFreeGameLoadedForRougeFight(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != FreeGameSceneName)
            return;

        SceneManager.sceneLoaded -= OnFreeGameLoadedForRougeFight;

        var fight = _pendingRougeFight;
        _pendingRougeFight = null;

        if (fight == null)
        {
            Debug.LogError("[RoomSelection] Pending roguelike fight was lost after scene load.");
            return;
        }

        if (PlayerManager.Instance == null)
        {
            Debug.LogError("[RoomSelection] PlayerManager.Instance is null after loading Free Game.");
            return;
        }

        if (RougeManager.Instance == null || RougeManager.Instance.rougePlayer == null)
        {
            Debug.LogError("[RoomSelection] RougeManager or rougePlayer is missing; ensure RougeManager persists (global) or exists in Free Game.");
            return;
        }

        PlayerManager.Instance.ReadSpacingData();
        PlayerManager.Instance.CreatePlayersForRougeFight(RougeManager.Instance.rougePlayer, fight);

        if (BattleManager.Instance == null)
            Debug.LogError("No BattleManager!");
        BattleManager.Instance.StartGame();
    }

    private static void LoadRougeScene(string sceneName)
    {
        if (WorkingOn.Instance != null)
            WorkingOn.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
