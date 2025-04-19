using Common.Data;
using JetBrains.Annotations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RoomManager : Singleton<RoomManager>
{
    public Dictionary<int ,Room> Rooms = new Dictionary<int ,Room>();
    public void Init()
    {
        Room.Instance.NewRoom();
    }

    public void CreateRoom(string name, List<CharacterSet> characters, string PassWord = "", int Number = 2, string AIType="",Privacy privacy= Privacy.Private,string Discription="")
    {
        int id = 1;
        Room.Instance.NewRoom();
        foreach (var kv in CharacterManager.Instance.Characters.Values)
        {
            if (kv.ID<0)
            {
                CharacterManager.Instance.Characters.Remove(kv.ID);
            }
        }
        BattleManager.Instance.NewBattle();
        int randomID;
        do
        {
            randomID = UnityEngine.Random.Range(1, 1000);
        } while (Rooms.ContainsKey(randomID));
        Room.Instance.room.ID = randomID;
        Room.Instance.room.Name = name;
        Room.Instance.room.NumberCount = Number;
        Room.Instance.room.PassWord = PassWord;
        Room.Instance.room.MasterID = User.Instance.ID;
        Room.Instance.room.MasterName = User.Instance.Name;
        Room.Instance.room.Description = Discription;
        foreach (var set in characters)
        {
            int hp = int.Parse(set.HP.text);
            int bullets = int.Parse(set.Bullets.text);
            int swords = int.Parse(set.Swords.text);
            for (int i = 1; i <= set.CurrentCount; i++)
            {
                CharacterData data = new CharacterData();
                data.Changed(int.Parse(set.HP.text), int.Parse(set.Bullets.text), int.Parse(set.Swords.text), 0,int.Parse(set.HP.text));
                BattleManager.Instance.Characters.Add(id++, data);
            }
        }
        Rooms.Add(randomID, Room.Instance);
        Room.Instance.Add(User.Instance.ID, characters[0]);
        if (privacy == Privacy.Private)
        {
            if (characters[0].CurrentCount == 1) characters.Remove(characters[0]);
            else characters[0].CurrentCount -= 1;
            AIMessageCreat.Instance.CustomizeCreate(characters);
            SceneManager.Instance.LoadScene("BattleScene");
            UIManager.Instance.currentScene = UIManager.Scene.BattleScene;
            
        }
    }

    public void AddCharacter(int roomid,int HeroId=0,int CharacterID=0,bool isAI=false)
    {
        if (Rooms.ContainsKey(roomid))
        {
            MessageBox.Show($"ȷ��Ҫ���뷿�䣺{roomid}��","����",MessageBoxType.Information ,"ȷ��", "ȡ��").OnYes = () => {
                //RoomService.
            };
        }
        else
        {
            MessageBox.Show("����Ų����ڣ������ǲ�������ˣ�","����",MessageBoxType.Error,"����");
        }
        Rooms.TryGetValue(roomid, out Room room);
        //if (room.room.NumberCount)
        //{

        //}
    }
    
}
