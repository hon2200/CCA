using Common.Data;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBookItem : BaseItem<ItemDefine>
{
    public bool isMouseOver = false;
    public float time = 0.5f;
    public delegate void OnCardChoose(int id);
    public static event OnCardChoose Choose;
    public Image This;
    public Image image;
    public Button button;
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI Hurt;
    public TextMeshProUGUI ATK;
    public TextMeshProUGUI Copywriting;
    public GameObject ShowHurt;
    public GameObject ShowCost;
    public override void SetItem(int Id, ItemDefine Bookitem)
    {
        base.SetItem(Id, Bookitem);
        this.name = Bookitem.Name;
        //this.Icon.overrideSprite = Resloader.Load<Sprite>(Bookitem.Icon);
        //this.Iconbig.overrideSprite = Icon.overrideSprite;
        if (Bookitem.ATK == 0)
        {
            ShowHurt.SetActive(false);
        }
        else
        {
            if (Hurt != null) Hurt.text = Bookitem.Hurt.ToString();
            if (ATK != null) ATK.text = Bookitem.ATK.ToString();
        }
        if (Cost != null)
        {
            Cost.text = Bookitem.Cost.ToString();
            if (Bookitem.Cost <= 0) ShowCost.SetActive(false);
        }
        if (Copywriting != null) Copywriting.text = Bookitem.Copywriting;
        if(Name!=null) Name.text = Bookitem.Name;
        button.onClick.AddListener(OnClick);
    }
    private void OnDestroy()
    {
            button.onClick.RemoveListener(OnClick);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        This.gameObject.SetActive(false);
        this.Name.transform.position += new Vector3(0, 73, 0);
        image.gameObject.SetActive(true);
        isMouseOver = true;
        StartCoroutine(LoadData());
    }

    public IEnumerator LoadData()
    {
        yield return new WaitForSeconds(time) ;
        if(isMouseOver) Iconbig?.gameObject.SetActive(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        This.gameObject.SetActive(true);
        this.Name.transform.position -= new Vector3(0, 73, 0);
        image.gameObject.SetActive(false);
        Iconbig?.gameObject.SetActive(false);
    }
    private void OnClick()
    {
        Choose?.Invoke(ItemID);
    }
}
