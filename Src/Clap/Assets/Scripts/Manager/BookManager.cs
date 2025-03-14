using Common.Data;
using GameServer.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : Singleton<BookManager>
{
    public void Init()
    {

    }

    public void ShowBook()
    {
        UIBook UIBook =UIManager.Instance.Show<UIBook>();
    }
}
