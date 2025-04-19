using Common.Data;
using JetBrains.Annotations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class UICreatRoom : MonoBehaviour
{
    public BalanceCount balanceCount;

    public TMP_InputField Name;
    public TMP_InputField PassWord;
    public TMP_InputField AIType;
    public TMP_InputField Discription;
    public GameObject group;
    public Toggle isPublic;
    private Dictionary<int, Team> teams = new Dictionary<int, Team>();
    private Privacy privacy;
    void Start()
    {

        Name.onEndEdit.AddListener((input) => Name.text = input);
        PassWord.onEndEdit.AddListener((input) => PassWord.text = input);
        AIType.onEndEdit.AddListener((input) => AIType.text = input);
        Discription.onEndEdit.AddListener((input) => Discription.text = input);
        isPublic.onValueChanged.AddListener(IsPublic);
        if (!group.activeSelf)
        {
            privacy = Privacy.Private;
        }
    }

   

    private void IsPublic(bool arg0)
    {
        privacy = arg0?Privacy.Public:Privacy.CanJoin;
        Debug.Log(privacy);
    }

    public void CreatRoom(List<CharacterSet> characterSets,int limit)
    {
            RoomManager.Instance.CreateRoom(this.Name.text, characterSets, this.PassWord.text, limit, AIType.text, privacy, Discription.text);
    }
}
