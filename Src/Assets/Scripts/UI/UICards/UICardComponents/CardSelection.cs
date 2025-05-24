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
        // 悬停效果：放大、旋转到正位、发光等
        transform.localScale = Vector3.one * 0.8f;
        rotation_origin = transform.rotation;
        transform.rotation = Quaternion.identity;
        Glow.SetActive(true);
        //移动到最上层
        order_origin = gameObject.GetComponent<CardUI>().cardCanvas.sortingOrder;
        position_origin = transform.position;
        gameObject.GetComponent<CardUI>().PromoteLayerTo(200);
        //移动到鼠标位置一些以便玩家看到
        transform.position += new Vector3(0, 0.2f, 0);
        OnHover = true;
    }

    // 鼠标离开卡牌范围时调用
    public void OnHoverExit()
    {
        // 恢复原状
        transform.localScale = Vector3.one * 0.6f;
        transform.rotation = rotation_origin;
        Glow.SetActive(false);
        gameObject.GetComponent<CardUI>().PromoteLayerTo(order_origin);
        transform.position = position_origin;
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