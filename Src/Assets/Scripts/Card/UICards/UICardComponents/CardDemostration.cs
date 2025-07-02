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
    // �����뿨�Ʒ�Χʱ����
    public void OnHoverEnter()
    {
        // ��ͣЧ�����Ŵ���ת����λ�������
        transform.localScale = Vector3.one * 0.5f;
        rotation_origin = transform.rotation;
        transform.rotation = Quaternion.identity;
        Glow.SetActive(true);
        //�ƶ������ϲ�
        order_origin = gameObject.GetComponent<CardUI>().cardCanvas.sortingOrder;
        position_origin = transform.position;
        gameObject.GetComponent<CardUI>().PromoteLayerTo(200);
        //��ʾĿ�꣺
        PlayerManager.Instance.Players.TryGetValue(target, out var player);
        Arrow.Instance.FromOriToDes(transform, player.gameObject.transform);
        onHover = true;
    }

    // ����뿪���Ʒ�Χʱ����
    public void OnHoverExit()
    {
        // �ָ�ԭ״
        transform.localScale = Vector3.one * 0.3f;
        transform.rotation = rotation_origin;
        Glow.SetActive(false);
        gameObject.GetComponent<CardUI>().PromoteLayerTo(order_origin);
        transform.position = position_origin;
        //ȡ��Ŀ����ʾ
        Arrow.Instance.DeActive();
        onHover = false;
    }
}
