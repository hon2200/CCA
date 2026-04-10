using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerManager : MonoSingleton<PlayerManager>
{
    public List<GameObject> EnemyPositionList;
    public List<GameObject> friendPositionList;
    public GameObject humanPosition;
    private Dictionary<Vector2, Player> availablePositions_enemy { get; set; }
    private Dictionary<Vector2, Player> availablePositions_friend { get; set; }
    private Vector2 availablePostion_human { get; set; }
    public int NextPlayerID { get; set; }
    public int AlivePlayerNumber { get; set; }

    public GameObject AIPrefab;
    public GameObject HumanPrefab;

    private Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private List<HumanPlayer> _humanPlayers = new List<HumanPlayer>();
    private List<AIPlayer> _aiPlayers = new List<AIPlayer>();
    private List<Player> _friendlyPlayers = new List<Player>();
    private List<Player> _hostilePlayers = new List<Player>();

    /// <summary>Read-only. Modify only via AddPlayer (Heroes path) or Level creation methods.</summary>
    public IReadOnlyDictionary<int, Player> Players => _players;
    /// <summary>First human player, or null if none. Used by skills/UI that assume a single human.</summary>
    public HumanPlayer HumanPlayer => _humanPlayers.Count > 0 ? _humanPlayers[0] : null;
    public IReadOnlyList<HumanPlayer> HumanPlayers => _humanPlayers;
    public IReadOnlyList<AIPlayer> AIPlayers => _aiPlayers;
    public IReadOnlyList<Player> FriendlyPlayers => _friendlyPlayers;
    public IReadOnlyList<Player> HostilePlayers => _hostilePlayers;
    public void Start()
    {
        ReadSpacingData();
    }
    public List<Player> GetAlivePlayers()
    {
        List<Player> liveones = new();
        foreach(var player in Players.Values)
        {
            if(player.status.life.Value != LifeStatus.Death)
                liveones.Add(player);
        }
        return liveones;
    }
    public bool ThereisAvailablePositions(bool enemy)
    {
        ClearDeadPeople();
        if(enemy)
        {
            foreach (var position in availablePositions_enemy)
            {
                if (position.Value == null)
                    return true;
            }
        }
        else
        {
            foreach (var position in availablePositions_friend)
            {
                if (position.Value == null)
                    return true;
            }
        }
        return false;
    }
    public List<Player> FindSomeone(string HeroID)
    {
        List<Player> specificPlayers = new();
        foreach(var player in Players.Values)
        {
            if (player.hero.ID == HeroID && player.status.life.Value == LifeStatus.Alive)
                specificPlayers.Add(player);
        }
        return specificPlayers;
    }

    #region Tutorial Things
    public void CreatingPlayers_BasedOnLevels(TutorialDefine tutorialDefine, int Wave = 0)
    {
        int friendCount = 0, enemyCount = 0, remains = 0;
        if (Wave == 0)
        {
            ClearAll();
            CreateHuman_BasedOnLevel(tutorialDefine);
        }

        else
            remains = ClearDeadPeople();
        if (tutorialDefine.EnemyList.Count > Wave)
        {
            enemyCount = tutorialDefine.EnemyList[Wave].Count;
            for (int i = 0; i < enemyCount; i++)
            {
                HeroDataBase.Instance.EnemyDictionary.TryGetValue(tutorialDefine.EnemyList[Wave][i], out var AI);
                if (AI != null)
                {
                    AddPlayer(false, false, AI, tutorialDefine);
                }
                else
                    Debug.Assert(false, "Can't find AI" + tutorialDefine.EnemyList[Wave][i]);
            }
        }
        int totalNumber = 1 + friendCount + enemyCount + remains;
        AlivePlayerNumber = totalNumber;
        //MyLog.PrintLoadedDictionary(Players, "MyLog/Loading/PlayerTable_Debug.txt");

    }
    private void CreateHuman_BasedOnLevel(TutorialDefine tutorialDefine)
    {
        HeroDataBase.Instance.HeroDictionary.TryGetValue(tutorialDefine.HeroId, out var heroDefine);
        AddPlayer(true, true, heroDefine, tutorialDefine);
    }
    #endregion

    #region Heroes Things
    /// <summary>
    /// Adds a player to Players and the appropriate Human/AI and Friendly/Hostile lists. Call only from inside PlayerManager.
    /// </summary>
    private void AddPlayerToCollections(Player player, bool isFriend, bool isHuman)
    {
        _players[player.ID_inGame] = player;
        if (isHuman)
            _humanPlayers.Add((HumanPlayer)player);
        else
            _aiPlayers.Add((AIPlayer)player);
        if (isFriend)
            _friendlyPlayers.Add(player);
        else
            _hostilePlayers.Add(player);
    }

    /// <summary>
    /// Removes and destroys all human players (e.g. to replace the chosen hero in setup).
    /// </summary>
    public void RemoveAllHumanPlayers()
    {
        var humansSnapshot = new List<HumanPlayer>(_humanPlayers);
        foreach (var human in humansSnapshot)
        {
            if (human == null) continue;
            _players.Remove(human.ID_inGame);
            _humanPlayers.Remove(human);
            _friendlyPlayers.Remove(human);
            Destroy(human.gameObject);
        }
    }

    /// <summary>
    /// Creates one player from hero data and adds them to Players and the correct Human/AI and Friendly/Hostile lists.
    /// </summary>
    public Player AddPlayer(bool isFriend, bool isHuman, HeroDefine heroDefine, TutorialDefine tutorialDefine = null)
    {
        int id;
        if (isHuman && _humanPlayers.Count == 0)
        {
            id = 1;
            if (NextPlayerID <= 1)
                NextPlayerID = 2;
        }
        else
        {
            id = NextPlayerID++;
            if (id == 1)
                id = NextPlayerID++;
        }
        if (isHuman)
        {
            var newPlayerObject = Instantiate(HumanPrefab, this.transform);
            newPlayerObject.name = "Player" + id;
            var newPlayer = newPlayerObject.GetComponent<HumanPlayer>();
            newPlayer.InitializePlayer(id, heroDefine, tutorialDefine);
            InitializeHumanPlayerSpace(newPlayer);
            AddPlayerToCollections(newPlayer, isFriend, isHuman: true);
            return newPlayer;
        }
        else
        {
            var newPlayerObject = Instantiate(AIPrefab, this.transform);
            newPlayerObject.name = "Player" + id;
            var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
            newPlayer.Initialize(id, heroDefine, tutorialDefine);
            newPlayer.isFriend = isFriend;
            ArrangeNewPlayer(newPlayer);
            AddPlayerToCollections(newPlayer, isFriend, isHuman: false);
            return newPlayer;
        }
    }

    public void CreatingPlayers_BasedOnGameSetting_Heroes()
    {
        // Hero and monsters are already added by DropDownController and MonsterAddController.
        if (_players.Count > 0)
            NextPlayerID = _players.Keys.Max() + 1;
        AlivePlayerNumber = _players.Count;
    }

    /// <summary>
    /// Build a fresh combat roster for a roguelike fight: 1 human from the selected run hero (fallback to blank),
    /// plus all enemies listed in the picked fight definition.
    /// </summary>
    public bool CreatePlayersForRougeFight(RougePlayer rougePlayer, RougeFightDefine pickedFight)
    {
        if (pickedFight == null)
        {
            Debug.LogError("[PlayerManager] CreatePlayersForRougeFight failed: pickedFight is null.");
            return false;
        }

        if (HeroDataBase.Instance == null)
        {
            Debug.LogError("[PlayerManager] HeroDataBase.Instance is null.");
            return false;
        }

        ClearAll();
        NextPlayerID = 1;
        var humanHeroDefine = new HeroDefine("Blank", 20)
        {
            Name = "Blank",
            Description = "Fallback hero for roguelike fight setup.",
            SkillIDList = new List<string>(),
        };

        var humanPlayer = AddPlayer(isFriend: true, isHuman: true, humanHeroDefine);
        if (rougePlayer?.Heroes != null && rougePlayer.Heroes.Count > 0)
            humanPlayer.SetHero(rougePlayer.Heroes[0]);

        if (pickedFight.Enemies != null)
        {
            foreach (var enemyId in pickedFight.Enemies)
            {
                if (string.IsNullOrEmpty(enemyId))
                    continue;

                if (!HeroDataBase.Instance.EnemyDictionary.TryGetValue(enemyId, out var enemyDefine) || enemyDefine == null)
                {
                    Debug.LogWarning($"[PlayerManager] Enemy id not found in EnemyDictionary: {enemyId}");
                    continue;
                }

                AddPlayer(isFriend: false, isHuman: false, enemyDefine);
            }
        }

        AlivePlayerNumber = _players.Count;
        if (_players.Count > 0)
            NextPlayerID = _players.Keys.Max() + 1;
        return _players.Count > 0;
    }
    #endregion
    #region Spacing Things
    public bool EnemyReachMaxNumber()
    {
        if (availablePositions_enemy.ContainsValue(null))
            return false;
        else
            return true;
    }
    public void ReadSpacingData()
    {
        availablePositions_enemy = new();
        availablePositions_friend = new();
        foreach(var spot in EnemyPositionList)
        {
            availablePositions_enemy.Add(new Vector2(spot.transform.localPosition.x, spot.transform.localPosition.y), null);
        }
        foreach(var spot in friendPositionList)
        {
            availablePositions_friend.Add(new Vector2(spot.transform.localPosition.x, spot.transform.localPosition.y), null);
        }
        availablePostion_human = new Vector2(humanPosition.transform.localPosition.x, humanPosition.transform.localPosition.y);
    }
    private void InitializeHumanPlayerSpace(Player human)
    {
        human.transform.localPosition = humanPosition.transform.localPosition;
    }
    private void ArrangeNewPlayer(Player newPlayer)
    {
        ClearDeadPeople();
        if(newPlayer is AIPlayer ai && ai.isFriend)
        {
            foreach (var kvp in availablePositions_friend)
            {
                if (kvp.Value == null)
                {
                    var pos = kvp.Key;
                    newPlayer.transform.localPosition = new Vector3(pos.x, pos.y, 1);
                    availablePositions_friend[pos] = newPlayer; // mark as occupied
                    return;
                }
            }
            Debug.Assert(false, "No Available positions");
        }
        //At this moment, the player haven't been added into Players;
        foreach (var kvp in availablePositions_enemy)
        {
            if (kvp.Value == null)
            {
                var pos = kvp.Key;
                newPlayer.transform.localPosition = new Vector3(pos.x, pos.y, 1);
                availablePositions_enemy[pos] = newPlayer; // mark as occupied
                return;
            }
        }
        Debug.Assert(false, "No Available positions");
    }
    private int ClearDeadPeople()
    {
        var deadPlayers = _players
                        .Where(p => p.Value.status.life.Value == LifeStatus.Death)
                        .Select(p => p.Key)
                        .ToList(); // Make a copy of the keys to remove

        foreach (var id in deadPlayers)
        {
            if (_players.TryGetValue(id, out var player))
            {
                Destroy(player.gameObject);
                _players.Remove(id);
                _humanPlayers.Remove(player as HumanPlayer);
                _aiPlayers.Remove(player as AIPlayer);
                _friendlyPlayers.Remove(player);
                _hostilePlayers.Remove(player);
                // Find its spot key(if any)
                Vector2? foundKey = null;
                if(player is AIPlayer ai && ai.isFriend)
                {
                    foreach (var kvp in availablePositions_friend)
                    {
                        if (kvp.Value == player)
                        {
                            foundKey = kvp.Key;
                        }
                    }
                    // Mark spot available again
                    if (foundKey.HasValue)
                        availablePositions_friend[foundKey.Value] = null;
                }
                foreach (var kvp in availablePositions_enemy)
                {
                    if (kvp.Value == player)
                    {
                        foundKey = kvp.Key;
                        break;
                    }
                }
                // Mark spot available again
                if (foundKey.HasValue)
                    availablePositions_enemy[foundKey.Value] = null;
            }
        }

        // Count remaining non-human players
        int remains = _players.Values.Count(p => p.playerType != PlayerType.Human);
        return remains;
    }

    private void ClearAll()
    {
        if (_players == null)
        {
            _players = new Dictionary<int, Player>();
            _humanPlayers = new List<HumanPlayer>();
            _aiPlayers = new List<AIPlayer>();
            _friendlyPlayers = new List<Player>();
            _hostilePlayers = new List<Player>();
            return;
        }

        List<Player> playersSnapshot = _players.Values.ToList(); // snapshot to avoid modification issues

        foreach (var player in playersSnapshot)
        {
            if (player == null) continue;

            // Safely destroy player object
            Destroy(player.gameObject);

            // Find its spot key for enemies
            Vector2? foundKey = null;
            foreach (var kvp in availablePositions_enemy)
            {
                if (kvp.Value == player)
                {
                    foundKey = kvp.Key;
                    break;
                }
            }

            // Mark spot available again
            if (foundKey.HasValue)
                availablePositions_enemy[foundKey.Value] = null;

            // Find its spot key for friends
            foreach (var kvp in availablePositions_friend)
            {
                if (kvp.Value == player)
                {
                    foundKey = kvp.Key;
                }
            }
            // Mark spot available again
            if (foundKey.HasValue)
                availablePositions_friend[foundKey.Value] = null;
        }

        // Clear player dictionary and lists
        _players.Clear();
        _humanPlayers.Clear();
        _aiPlayers.Clear();
        _friendlyPlayers.Clear();
        _hostilePlayers.Clear();
    }
    #endregion
}
