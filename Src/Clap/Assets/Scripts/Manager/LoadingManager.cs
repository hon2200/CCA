using GameServer.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingManager : MonoBehaviour
{
    IEnumerator Start()
    {
        RoomManager.Instance.Init();
        BookManager.Instance.Init();
        CharacterManager.Instance.Init();
        yield return DataManager.Instance.LoadData();
    }
}
