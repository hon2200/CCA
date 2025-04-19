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

public class BaseItem<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler where T : class
{
    public Image Icon;
    public Image Iconbig;
    public TextMeshProUGUI Name;
    public int ItemID { get; set; }
    public T Item;

    public virtual void Awake()
    {

    }


    public virtual void SetItem(int Id, T data)
    {
        ItemID = Id;
        Item = data;
    }

    public virtual void OnPointerEnter(PointerEventData eventData) => Iconbig?.gameObject.SetActive(true);

    public virtual void OnPointerExit(PointerEventData eventData) => Iconbig?.gameObject.SetActive(false);
}
