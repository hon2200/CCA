using Common.Data;
using GameServer.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIBookItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public Image Icon;
    public Image Iconbig;
    public int BookItemID { get; set; }
    private UIBook book;
    private ItemDefine BookItem;
    private UIBookmessage bookmessage;
    public void SetBookItem(int Id, ItemDefine Bookitem, UIBook owner)
    {
        this.book = owner;
        this.BookItemID = Id;
        this.BookItem = Bookitem;
        this.Icon.overrideSprite = Resloader.Load<Sprite>(Bookitem.Icon);
        this.Iconbig.overrideSprite = Resloader.Load<Sprite>(Bookitem.Icon);
        this.bookmessage = owner.message;
        this.name = Bookitem.Name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Iconbig.gameObject.SetActive(true);
        bookmessage.UpdateInfo(BookItem.Icon, BookItem.Name, BookItem.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Iconbig.gameObject.SetActive(false);
    }
}
