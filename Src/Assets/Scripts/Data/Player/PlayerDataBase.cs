using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerDataBase : Singleton<PlayerDataBase>
{
    public string path;
    public Dictionary<int, PlayerDefine> playerDictionary;
    // ����ֵ䣬��������������ݿ�
    public Dictionary<int, PlayerDefine> PlayerDictionary { get; set; }
    //�����������
    public void LoadingPlayers()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Player/Player.json");
        playerDictionary = JsonLoader.DeserializeObject<Dictionary<int, PlayerDefine>>(path);
        //��ӡ�ж��ൽ��־
        Log.PrintLoadedDictionary(PlayerDictionary,"Log/Loading/Playerlog.txt");
    }
}
