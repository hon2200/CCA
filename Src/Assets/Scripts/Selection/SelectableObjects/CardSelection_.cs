using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

//这个类只负责卡牌出入时需要具体做哪些事情
public class CardSelection_ : HoverableBase
{
    public GameObject Glow;
    private int order_origin;
    private CardUI cardUI;

    [Header("悬浮音效配置")]
    [Tooltip("卡牌悬浮音量（-1使用AudioManager全局音量）")]
    [Range(-1f, 1f)] public float cardHoverVolume = -1f;

    void Awake() => cardUI = GetComponent<CardUI>();

    public override void OnHoverEnter(Vector3? scaleMultiplier, Quaternion? rotationOffset, Vector3? positionOffset,
                                      Quaternion? rotationFinal, Vector3? positionFinal)
    {
        base.OnHoverEnter(scaleMultiplier, rotationOffset, positionOffset, rotationFinal, positionFinal);

        Glow.SetActive(true);
        order_origin = cardUI.cardCanvas.sortingOrder;
        cardUI.PromoteLayerTo(200);

        // 新增：播放悬浮音效（带冷却逻辑）
        PlayCardHoverSound();
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
        Glow.SetActive(false);
        cardUI.PromoteLayerTo(order_origin);
    }


    /// 播放卡牌悬浮音效（复用AudioManager逻辑）
    private void PlayCardHoverSound()
    {
        // 校验AudioManager是否存在
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager实例不存在，无法播放卡牌悬浮音效！");
            return;
        }

        // 自定义音量生效（如果设置了的话）
        if (cardHoverVolume >= 0f && cardHoverVolume <= 1f)
        {
            AudioManager.Instance.SetUIHoverVolume(cardHoverVolume);
        }

        // 调用AudioManager播放悬浮音效
        AudioManager.Instance.PlayUIHoverSound();
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


