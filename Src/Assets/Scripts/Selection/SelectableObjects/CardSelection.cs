using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

//这个类只负责卡牌出入时需要具体做哪些事情
public class CardSelection : HoverableBase
{
    public GameObject Glow;
    private int order_origin;
    private CardUI cardUI;

    void Awake() => cardUI = GetComponent<CardUI>();

    public override void OnHoverEnter(Vector3? scaleMultiplier, Quaternion? rotationOffset, Vector3? positionOffset,
                                      Quaternion? rotationFinal, Vector3? positionFinal)
    {
        base.OnHoverEnter(scaleMultiplier, rotationOffset, positionOffset, rotationFinal, positionFinal);

        Glow.SetActive(true);
        order_origin = cardUI.cardCanvas.sortingOrder;
        cardUI.PromoteLayerTo(200);
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
        Glow.SetActive(false);
        cardUI.PromoteLayerTo(order_origin);
    }

    public bool HaveTarget()
    {
        string ID = GetComponent<RunTimeCard>().actionDefine.ID;
        ActionDataBase.Instance.ActionDictionary.TryGetValue(ID, out var action);
        switch (action.TargetType)
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


