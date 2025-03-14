using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIBookmessage : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image item;

    internal void UpdateInfo(string icon, string name, string description)
    {
        if (this.title != null) this.title.text =name;
        if (this.description != null) this.description.text = description;
        if (this.item != null) this.item.overrideSprite = Resloader.Load<Sprite>(icon);
    }
}
