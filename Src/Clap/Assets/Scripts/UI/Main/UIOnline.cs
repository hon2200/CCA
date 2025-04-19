using Common.Data;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UIOnline : UIWindow
{                                    //后续升级为对象池！！！暂且搁浅
    public Button sreach;
    public string sreachText;


    public TextMeshProUGUI page;
    private Text currentpage;
    private Text totalpage;

    [SerializeField] private GameObject RoomItem;
    public TMP_Dropdown modeDropdown;
    public TMP_InputField InputField;
    Dictionary<int ,RoomDefine> currentRooms = new Dictionary<int, RoomDefine>();
    void Start()
    {
        modeDropdown.options = new List<TMP_Dropdown.OptionData> {
            new TMP_Dropdown.OptionData("全部"),
            new TMP_Dropdown.OptionData("单人对战"),
            new TMP_Dropdown.OptionData("团队对战"),
            new TMP_Dropdown.OptionData("单人英雄"),
            new TMP_Dropdown.OptionData("团队英雄")
        };
        InputField.onValueChanged.AddListener(Input);
        displayAII();
    }
    public void displayAII()
    {
        OnClear();
        foreach (var kv in RoomManager.Instance.Rooms)
        {
            GameObject go = Instantiate(RoomItem, Root.transform);
            UIRoomItem ui = go.GetComponent<UIRoomItem>();
            ui.SetItem(kv.Key, kv.Value.room);
            currentRooms.Add(kv.Key, kv.Value.room);
        }
    }
    public void Input(string searchInput)
    {
        sreachText = searchInput;
    }
    public void OnSearch()
    {
        if (string.IsNullOrWhiteSpace(sreachText) || string.IsNullOrEmpty(sreachText))
        {
            displayAII();
            return;
        }
        else
        {
            OnClear();
            int number;
            bool success = int.TryParse(sreachText, out number);
            foreach (var room in RoomManager.Instance.Rooms)
            {
                if ((success == true & room.Value.room.ID == number) || room.Value.room.Name.Contains(sreachText, StringComparison.OrdinalIgnoreCase) || (success == true & room.Value.room.MasterID == number) || room.Value.room.MasterName.Contains(sreachText, StringComparison.OrdinalIgnoreCase))
                {
                    currentRooms.Add(room.Key, room.Value.room);
                }
            }
            Ondisplay();
        }
    }

    public void OnClear()
    {
        currentRooms.Clear();
        foreach (Transform kv in Root.transform)
        {
            Destroy(kv.gameObject);
        }
    }

    public void Ondisplay()
    {
        foreach (var kv in currentRooms)
        {
            GameObject go = Instantiate(RoomItem, Root.transform);
            UIRoomItem ui = go.GetComponent<UIRoomItem>();
            ui.SetItem(kv.Key, kv.Value);
        }

    }
}
