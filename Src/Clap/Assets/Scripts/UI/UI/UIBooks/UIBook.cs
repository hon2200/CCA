
using Common.Data;
using GameServer.Managers;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Unity.VisualScripting;
using System.Collections;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using static UnityEditor.Progress;
using TMPro;

public class UIBook : UIWindow
{
    // UI组件绑定
    [SerializeField] private UIInputBox Input;

    // 预制体引用
    [SerializeField] private GameObject bookItem; 
    [SerializeField] private TextMeshProUGUI sreachcontent;
    [SerializeField] private Button Button;
    public UIBookmessage message;

    public void Start()
    {
        //进入UIBook
        StartCoroutine(InitItems());
    }

    IEnumerator InitItems()//初始化
    {
        sreachcontent.text = null;
        Button.gameObject.SetActive(false);
        foreach (var kv in DataManager.Instance.Items)
        {
            GameObject go = Instantiate(bookItem, Root);
            UIBookItem ui = go.GetComponent<UIBookItem>();
            ui.SetBookItem(kv.Key, kv.Value, this);
        }
        yield return null;
    }

    public void OnSreach()
    {
            InputBox.Show("输入要查询的卡牌").OnSubmit += SreachCore;
    }

    private bool SreachCore(string input, out string tips)
    {
        tips = "";
        string CoreName =input;
        foreach (Transform item in Root)
        {
            if (!item.name.Contains(CoreName, StringComparison.OrdinalIgnoreCase))
            {
                item.gameObject.SetActive(false); // 隐藏不匹配的项
            }
        }
        sreachcontent.text = input;
        Button.gameObject.SetActive(true);
        return true;
    }
    public void ClearSreach()
    {
        sreachcontent.text = null;
        Button.gameObject.SetActive(false);
        foreach (Transform item in Root)
        {
            item.gameObject.SetActive(true);
        }
    }
}