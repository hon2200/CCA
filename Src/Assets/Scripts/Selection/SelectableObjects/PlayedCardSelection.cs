using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayedCardSelection : CardSelection
{
    public int target;
    public override void OnHoverEnter(Vector3? scaleMultiplier, Quaternion? rotationOffset, Vector3? positionOffset, Quaternion? rotationFinal, Vector3? positionFinal)
    {
        base.OnHoverEnter(scaleMultiplier, rotationOffset, positionOffset, rotationFinal, positionFinal);
        if(base.HaveTarget())
        {
            //显示目标：
            PlayerManager.Instance.Players.TryGetValue(target, out var player);
            Arrow.Instance.FromOriToDes(transform, player.gameObject.transform);
        }
    }

    public override void OnHoverExit()
    {
        base.OnHoverExit();
        //取消目标显示
        Arrow.Instance.DeActive();
    }

}
