
using Common.Data;
using GameServer.Managers;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

public class UIBook : UIWindow
{
    
    public BattleItem BattleBook;
    // UI组件绑定
    [SerializeField] private UIInputBox Input;

    // 预制体引用
    public GameObject bookItem; 
    [SerializeField] private TextMeshProUGUI sreachcontent;
    [SerializeField] private Button Button;
    [SerializeField] private GameObject Left;
    [SerializeField] private GameObject right;

    public Dictionary<int,UIBookItem> uIBookItems = new Dictionary<int, UIBookItem>();
    ItemType type=ItemType.补给;

    public void Start()
    {
        BattleItem.Choosed += Selected;
        if (UIManager.Instance.currentScene==UIManager.Scene.BattleScene)
        {
            right.gameObject.SetActive(false);
            if (Left!=null)Left.SetActive(false);
            BattleBook.uIBookItems = this.uIBookItems;

        }else
        {
            InitItems();
        }
    }

    public void OnDestroy()
    {
        BattleItem.Choosed -= Selected;
    }
    public void Selected()
    {
        right.gameObject.SetActive(false);
    }
     public void InitItems()//初始化
    {
        foreach (var kv in DataManager.Instance.Items)
        {
            if (RoomCheck(kv.Value))//方法调用
            {
                GameObject go = Instantiate(bookItem, Root.transform);
                UIBookItem ui = go.GetComponent<UIBookItem>();
                ui.SetItem(kv.Key, kv.Value);
                uIBookItems.Add(kv.Key,ui);
            }
        }
    }
    public bool RoomCheck(ItemDefine kv)
    {
        if (kv.ItemType == ItemType.货币) return false;
        ///是否需要未解锁卡牌
        return true;
    }
    public void OnSreach()
    {
            InputBox.Show("输入要查询的卡牌").OnSubmit += SreachCore;
    }
    private bool SreachCore(string input, out string tips)
    {
        tips = "";
        string CoreName =input;
        foreach (KeyValuePair<int, UIBookItem> pair in uIBookItems)
        {
            UIBookItem item = pair.Value;
            if (!item.Item.ID.ToString().Contains(CoreName, StringComparison.OrdinalIgnoreCase) && 
                !item.Item.Name.Contains(CoreName, StringComparison.OrdinalIgnoreCase) &&
                !item.Item.Description.Contains(CoreName, StringComparison.OrdinalIgnoreCase) &&
                !item.Item.ItemType.ToString().Contains(CoreName, StringComparison.OrdinalIgnoreCase))
            {
                item.gameObject.SetActive(false);
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
        foreach (KeyValuePair<int, UIBookItem> pair in uIBookItems)
        {
            UIBookItem item = pair.Value;
            item.gameObject.SetActive(true);
        }
    }
    public void TypeSelect(int type)
    {
        if (UIManager.Instance.currentScene == UIManager.Scene.BattleScene)
        {
            if(BattleItem.Instance.Choose) return;
            right.SetActive(true);
            BattleItem.Instance.TypeSelect(type);
            return;
        }
        ItemType tt=(ItemType)Enum.ToObject(typeof(ItemType), type);
        if (this.type == tt)
        {
            ClearSreach();
            this.type = ItemType.None;
            return;
        }
        else
        {
            this.type = tt;
        }
        foreach (KeyValuePair<int, UIBookItem> pair in uIBookItems)
        {
            UIBookItem item = pair.Value;
            if (type == -2)
            {
                if (item.Item.ItemType == ItemType.近战 || item.Item.ItemType == ItemType.远程)
                {
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
            else
            {
                if (item.Item.ItemType != tt)
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    item.gameObject.SetActive(true);
                }
            }
                
        }
    }
}