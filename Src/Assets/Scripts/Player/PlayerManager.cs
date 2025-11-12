using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerManager : MonoSingleton<PlayerManager>
{
    private Dictionary<Vector2,Player> availablePositions { get; set; }
    private int maxPlayerCount { get { return 6; } }
    public int NextPlayerID { get; set; }
    public int AlivePlayerNumber { get; set; }

    public GameObject AIPrefab;
    public GameObject HumanPrefab;
    public Dictionary<int, Player> Players;
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
    public bool ThereisAvailablePositions()
    {
        ClearDeadPeople();
        foreach(var position in availablePositions)
        {
            if (position.Value == null)
                return true;
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
            foreach(var avai in availablePositions)
            {
                if (avai.Value == null)
                    Debug.Log(avai);
            }
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
    public bool ReachMaxNumber()
    {
        if (AlivePlayerNumber >= maxPlayerCount)
            return true;
        else
            return false;
    }
    public void ReadSpacingData()
    {
        if (!PlayerSpacingDataBase.Instance.playerSpacingDictionary.TryGetValue(maxPlayerCount, out var spacingData))
        {
            Debug.LogError($"No spacing data found for player count: {maxPlayerCount}");
            return;
        }

        availablePositions = new();
        for (int i = 0; i < spacingData.Player_X.Count; i++)
        {
            availablePositions.Add(new Vector2(spacingData.Player_X[i], spacingData.Player_Y[i]), null);
        }
    }
    private void InitializeHumanPlayerSpace(Player human)
    {
        human.transform.localPosition = new Vector3(7.5f, -4.5f, 1);
    }
    private void ArrangeNewPlayer(Player newPlayer)
    {
        ClearDeadPeople();
        //At this moment, the player haven't been added into Players;
        foreach (var kvp in availablePositions)
        {
            if (kvp.Value == null)
            {
                var pos = kvp.Key;
                newPlayer.transform.localPosition = new Vector3(pos.x, pos.y, 1);
                availablePositions[pos] = newPlayer; // mark as occupied
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

                // Find its position key(if any)
                Vector2? foundKey = null;
                foreach (var kvp in availablePositions)
                {
                    if (kvp.Value == player)
                    {
                        foundKey = kvp.Key;
                        break;
                    }
                }

                // Mark position available again
                if (foundKey.HasValue)
                    availablePositions[foundKey.Value] = null;
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

            // Find its position key (if any)
            Vector2? foundKey = null;
            foreach (var kvp in availablePositions)
            {
                if (kvp.Value == player)
                {
                    foundKey = kvp.Key;
                    break;
                }
            }

            // Mark position available again
            if (foundKey.HasValue)
                availablePositions[foundKey.Value] = null;
        }

        // Clear player dictionary
        Players.Clear();
    }
    #endregion
}
