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
    public Dictionary<int, Player> Players;
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

    #region AI Things
    public void CreateCurrentLevelWave()
    {
        Debug.Log("Level ID" + LevelManager.Instance.Level.ID + "Wave " + LevelManager.Instance.Level.Wave);
        CreatingPlayers_BasedOnLevels(LevelManager.Instance.GetCurrentLevel(), LevelManager.Instance.Level.Wave);
    }
    public void CreatingPlayers_BasedOnLevels(LevelDefine levelDefine, int Wave)
    {
        int friendCount = 0, enemyCount = 0, remains = 0;
        if (Wave == 0)
        {
            ClearAll();
            Player newPlayerH = CreateHuman_BasedOnLevel(levelDefine);
        }

        else
            remains = ClearDeadPeople();
        if (levelDefine.FriendList.Count > Wave)
        {
            friendCount = levelDefine.FriendList[Wave].Count;
            for (int j = 0; j < friendCount; j++)
            {
                AIDataBase.Instance.AIDictionary.TryGetValue(levelDefine.FriendList[Wave][j], out var AI);
                if (AI != null)
                {
                    Player newPlayer = CreateAI(AI, true, levelDefine);
                }

                else
                    Debug.Assert(false, "Can't find AI" + levelDefine.FriendList[Wave][j]);
            }
        }
        if (levelDefine.EnemyList.Count > Wave)
        {
            enemyCount = levelDefine.EnemyList[Wave].Count;
            for (int i = 0; i < enemyCount; i++)
            {
                AIDataBase.Instance.AIDictionary.TryGetValue(levelDefine.EnemyList[Wave][i], out var AI);
                if (AI != null)
                {
                    Player newPlayer = CreateAI(AI, false, levelDefine);
                }

                
                else
                    Debug.Assert(false, "Can't find AI" + levelDefine.EnemyList[Wave][i]);
            }
        }
        int totalNumber = 1 + friendCount + enemyCount + remains;
        AlivePlayerNumber = totalNumber;
        //MyLog.PrintLoadedDictionary(Players, "MyLog/Loading/PlayerTable_Debug.txt");

    }
    public Player CreateAI(AIDefine aIDefine, bool isFriend, LevelDefine levelDefine)
    {
        int ID_inGame = NextPlayerID;
        var newPlayerObject = Instantiate(AIPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
        newPlayer.Initialize(ID_inGame, aIDefine, isFriend, levelDefine);
        ArrangeNewPlayer(newPlayer);
        NextPlayerID++;
        Players.Add(newPlayer.ID_inGame, newPlayer);
        return newPlayer;
    }
    private Player CreateHuman_BasedOnLevel(LevelDefine level)
    {
        int ID_inGame = NextPlayerID;
        var newPlayerObject = Instantiate(HumanPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        var newPlayer = newPlayerObject.GetComponent<HumanPlayer>();
        newPlayer.InitializePlayer(ID_inGame, level);
        InitializeHumanPlayerSpace(newPlayer);
        NextPlayerID++;
        Players.Add(newPlayer.ID_inGame, newPlayer);
        return newPlayer;
    }
    #endregion

    #region Heroes Things
    public void CreatingPlayers_BasedOnGameSetting_Heroes()
    {
        if (GameSetting.Instance == null)
        {
            return;
        }
        Players = new Dictionary<int, Player>();
        List<HeroDefine> heroDefines = new();

        foreach (var heroID in GameSetting.Instance.HeroIDDictionary)
        {
            HeroDataBase.Instance.HeroDictionary.TryGetValue(heroID, out var heroDefine);
            if (heroDefine != null)
                heroDefines.Add(heroDefine);
            else
                Debug.Assert(false, "Can't fine Hero");
        }
        int totalNumber = heroDefines.Count;
        var newHumanPlayer = CreateHumanHero(1, heroDefines[0]);
        Players.Add(1, newHumanPlayer);
        for (int i = 2; i <= totalNumber; i++)
        {
            var newPlayer = CreateAIHero(i, heroDefines[i - 1]);
            Players.Add(i, newPlayer);
        }
        AlivePlayerNumber = totalNumber;
        //MyLog.PrintLoadedDictionary(Players, "MyLog/Loading/PlayerTable_Debug.txt");
    }
    private Player CreateAIHero(int ID_inGame, HeroDefine heroDefine)
    {

        var newPlayerObject = Instantiate(AIPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;

        var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
        newPlayer.Initialize(ID_inGame, heroDefine);
        ArrangeNewPlayer(newPlayer);
        Players.Add(newPlayer.ID_inGame, newPlayer);
        return newPlayer;
    }
    private Player CreateHumanHero(int ID_inGame, HeroDefine heroDefine)
    {

        var newPlayerObject = Instantiate(HumanPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;

        var newPlayer = newPlayerObject.GetComponent<HumanPlayer>();
        newPlayer.InitializePlayer(ID_inGame, heroDefine);
        InitializeHumanPlayerSpace(newPlayer);
        Players.Add(newPlayer.ID_inGame, newPlayer);
        return newPlayer;
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
        human.transform.localPosition = new Vector3(7.5f, -4.5f, 1);
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
        var deadPlayers = Players
                        .Where(p => p.Value.status.life.Value == LifeStatus.Death)
                        .Select(p => p.Key)
                        .ToList(); // Make a copy of the keys to remove

        foreach (var id in deadPlayers)
        {
            if (Players.TryGetValue(id, out var player))
            {
                Destroy(player.gameObject);
                Players.Remove(id);
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
        int remains = Players.Values.Count(p => p.playerType != PlayerType.Human);
        return remains;
    }

    private void ClearAll()
    {
        if (Players == null)
        {
            Players = new();
            return;
        }

        List<Player> playersSnapshot = Players.Values.ToList(); // snapshot to avoid modification issues

        foreach (var player in playersSnapshot)
        {
            if (player == null) continue;

            // Safely destroy player object
            Destroy(player.gameObject);

            // Find its spot key (if any)
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
        }

        // Clear player dictionary
        Players.Clear();
    }
    #endregion
}
