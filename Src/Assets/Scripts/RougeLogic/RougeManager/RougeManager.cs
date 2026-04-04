using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



/// <summary>

/// Roguelike meta + run state (<see cref="rougePlayer"/>). Card / hero / relic offer UI is handled by

/// <see cref="CardOfferManager"/>, <see cref="HeroOfferManager"/>, and <see cref="RelicOfferManager"/>.

/// </summary>

public partial class RougeManager : MonoSingleton<RougeManager>

{

    public static RoomID PendingRoomID { get; private set; } = RoomID.Undecided;

    public static string PendingFightID { get; private set; }



    public RougePlayer rougePlayer;

    [Header("General UI")]

    [SerializeField] private GameObject BonusRoomPanel;



    [Header("Offer UI (optional; otherwise resolved via GetComponentInChildren)")]

    [SerializeField] private CardOfferManager cardOfferManager;

    [SerializeField] private HeroOfferManager heroOfferManager;

    [SerializeField] private RelicOfferManager relicOfferManager;



    public static void SetPendingRoom(RoomID roomID)

    {

        PendingRoomID = roomID;

    }

    public static void SetPendingFight(string fightID)

    {

        PendingFightID = fightID;

    }



    private void Awake()

    {

        rougePlayer = new RougePlayer();

        rougePlayer.Relics.OnListChanged = (list, message) =>

        {

            if (RelicDisplay.Instance != null)

                RelicDisplay.Instance.RefreshDisplay();

        };



        if (cardOfferManager == null)

            cardOfferManager = GetComponentInChildren<CardOfferManager>(true);

        if (heroOfferManager == null)

            heroOfferManager = GetComponentInChildren<HeroOfferManager>(true);

        if (relicOfferManager == null)

            relicOfferManager = GetComponentInChildren<RelicOfferManager>(true);

    }



    private void Start()

    {

        if (SceneManager.GetActiveScene().name != "Free Game")

            return;



        bool isFightRoom = PendingRoomID == RoomID.Minion

            || PendingRoomID == RoomID.Elite

            || PendingRoomID == RoomID.Boss;

        if (BonusRoomPanel != null)

            BonusRoomPanel.SetActive(!isFightRoom);



        if (isFightRoom)

        {

            StartPendingFightBattle();

            return;

        }



        switch (PendingRoomID)

        {

            case RoomID.Tavern:

                heroOfferManager?.DisplayThreeRandomHeroesNotOwned(clearContainer: true);

                break;

            case RoomID.SoulFountain:

                cardOfferManager?.DisplayThreeRandomCardsNotInAvailableActions(clearParent: true);

                break;

            case RoomID.SacredCemetery:

                relicOfferManager?.DisplayThreeRandomRelicsNotOwned(clearContainer: true);

                break;

        }

    }



    private void StartPendingFightBattle()

    {

        if (RougeFightsDatabase.Instance == null)

        {

            Debug.LogError("[RougeManager] RougeFightsDatabase.Instance is null.");

            return;

        }

        if (PlayerManager.Instance == null)

        {

            Debug.LogError("[RougeManager] PlayerManager.Instance is null.");

            return;

        }

        if (BattleManager.Instance == null)

        {

            Debug.LogError("[RougeManager] BattleManager.Instance is null.");

            return;

        }



        var db = RougeFightsDatabase.Instance;

        if (db.FightDictionary == null || db.FightDictionary.Count == 0)

            db.LoadingFights();



        RougeFightDefine pickedFight = null;

        if (!string.IsNullOrEmpty(PendingFightID) && db.FightDictionary != null)

            db.FightDictionary.TryGetValue(PendingFightID, out pickedFight);



        if (pickedFight == null)

        {

            string fightType = PendingRoomID.ToString();

            pickedFight = db.PickRandomFightByType(fightType);

        }



        if (pickedFight == null)

        {

            Debug.LogError($"[RougeManager] Cannot resolve fight for room: {PendingRoomID}");

            return;

        }



        bool created = PlayerManager.Instance.CreatePlayersForRougeFight(rougePlayer, pickedFight);

        if (!created)

        {

            Debug.LogError("[RougeManager] Failed to create players for pending fight.");

            return;

        }



        BattleManager.Instance.StartBattle();

        PendingFightID = null;

    }

}

