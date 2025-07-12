using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

//这个类只负责卡牌出入时需要具体做哪些事情
public class CardSelection : MonoBehaviour, IHoverable
{
    public GameObject Glow;
    private Quaternion rotation_origin;
    private Vector3 position_origin;
    private int order_origin;
    public bool OnHover;

    public bool IsOnHover()
    {
        return OnHover;
    }
    // 鼠标进入卡牌范围时调用
    public void OnHoverEnter()
    {
        Glow.SetActive(true);
        //移动到最上层
        order_origin = gameObject.GetComponent<CardUI>().cardCanvas.sortingOrder;
        gameObject.GetComponent<CardUI>().PromoteLayerTo(200);
        OnHover = true;
    }

    // 鼠标离开卡牌范围时调用
    public void OnHoverExit()
    {
        Glow.SetActive(false);
        gameObject.GetComponent<CardUI>().PromoteLayerTo(order_origin);
        OnHover = false;
    }

    public bool HaveTarget()
    {
        string ID = GetComponent<RunTimeCard>().actionDefine.ID;
        ActionDataBase.Instance.ActionDictionary.TryGetValue(ID, out var action);
        switch(action.TargetType)
        {
            case TargetType.Self:
                return false;
            case TargetType.Enemy:
                return true;
            default:
                Debug.Assert(false, "Wrong Target Type");
                return false;
        }
    }
}