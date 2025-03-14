using GameServer.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingManager : MonoBehaviour
{
    // start is called before the first frame update
    IEnumerator Start()
    {

        BookManager.Instance.Init();
        yield return DataManager.Instance.LoadData();
    }
}
