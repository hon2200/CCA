using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerSelection : MonoBehaviour, IHoverable
{
    //即将选择高亮
    public GameObject Glow;
    //确认高亮
    public GameObject Glow2;
    public bool onHover;
    public bool IsOnHover()
    {
        return onHover;
    }
    public void OnHoverEnter()
    {
        onHover = true;
        // 悬停效果：发光
        Glow.SetActive(true);
    }
    public void OnHoverExit()
    {
        onHover = false;
        // 悬停效果：发光
        Glow.SetActive(false);
    }
    public void OnSelect()
    {
        Glow.SetActive(false);
        Glow2.SetActive(true);
    }
    public void OnUnSelect()
    {
        Glow2.SetActive(false);
    }
}
