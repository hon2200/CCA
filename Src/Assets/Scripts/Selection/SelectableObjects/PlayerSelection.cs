using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSelection : HoverableBase
{
    //是否能成为目标
    public bool CanbeSelected = true;
    //即将选择高亮
    public GameObject ReadyToSelectGlow;
    //确认高亮
    public GameObject SelectionCompletedGlow;
    #region Interface Realization

    public override void OnHoverEnter(Vector3? scaleMultiplier, Quaternion? rotationOffset, Vector3? positionOffset,
                                  Quaternion? rotationFinal, Vector3? positionFinal)
    {
        base.OnHoverEnter(scaleMultiplier, rotationOffset, positionOffset, rotationFinal, positionFinal);
        if (CanbeSelected)
        {
            onHover = true;
            // 悬停效果：发光
            ReadyToSelectGlow.SetActive(true);
        }
    }
    public override void OnHoverExit()
    {
        base.OnHoverExit();
        // 悬停效果：发光
        ReadyToSelectGlow.SetActive(false);
    }
    #endregion

    #region Complementary Functions
    public void OnSelect()
    {
        ReadyToSelectGlow.SetActive(false);
        SelectionCompletedGlow.SetActive(true);
    }
    public void OnUnSelect()
    {
        SelectionCompletedGlow.SetActive(false);
    }
    public void DimAllGlows()
    {
        ReadyToSelectGlow.SetActive(false);
        SelectionCompletedGlow.SetActive(false);
    }
    #endregion


    [SerializeField] private GameObject _glowLayer;

}
