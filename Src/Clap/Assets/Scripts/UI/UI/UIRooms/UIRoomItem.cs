using Common.Data;
using GameServer.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Character;
using static UnityEditor.Progress;

public class UIRoomItem : BaseItem<RoomDefine>
{
    public override void SetItem(int Id, RoomDefine Roomitem)
    {
        base.SetItem(Id, Roomitem);
        this.name = Roomitem.Name;
        Name.text = this.name;
        //this.Icon.overrideSprite = Resloader.Load<Sprite>(Roomitem.Icon);
        //this.Iconbig.overrideSprite = this.Icon.overrideSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }
}
