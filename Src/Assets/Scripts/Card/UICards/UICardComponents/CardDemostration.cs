using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDemostration : MonoBehaviour, IHoverable
{
    public GameObject Glow;
    private Quaternion rotation_origin;
    private Vector3 position_origin;
    private int order_origin;
    public int target;
    public bool onHover;

    public bool IsOnHover()
    {
        return onHover;
    }
    // 鼠标进入卡牌范围时调用
    public void OnHoverEnter()
    {
        // 悬停效果：放大、旋转到正位、发光等
        transform.localScale = Vector3.one * 0.5f;
        rotation_origin = transform.rotation;
        transform.rotation = Quaternion.identity;
        Glow.SetActive(true);
        //移动到最上层
        order_origin = gameObject.GetComponent<CardUI>().cardCanvas.sortingOrder;
        position_origin = transform.position;
        gameObject.GetComponent<CardUI>().PromoteLayerTo(200);
        //显示目标：
        PlayerManager.Instance.Players.TryGetValue(target, out var player);
        Arrow.Instance.FromOriToDes(transform, player.gameObject.transform);
        onHover = true;
    }

    // 鼠标离开卡牌范围时调用
    public void OnHoverExit()
    {
        // 恢复原状
        transform.localScale = Vector3.one * 0.3f;
        transform.rotation = rotation_origin;
        Glow.SetActive(false);
        gameObject.GetComponent<CardUI>().PromoteLayerTo(order_origin);
        transform.position = position_origin;
        //取消目标显示
        Arrow.Instance.DeActive();
        onHover = false;
    }
}
