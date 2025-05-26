using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//实现对射线碰撞检测以及鼠标位置的处理
public static class MouseAndRayUtil
{
    public static void FollowMouse(Transform transform, float z_varient = 0)
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePos = Input.mousePosition;
        // 将鼠标在屏幕上的位置转换为世界空间中的位置
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new(mousePos.x, mousePos.y, Camera.main.transform.position.z));
        //赋给物体
        transform.position = new(worldPos.x, worldPos.y, transform.position.z + z_varient);
    }
    public static bool Hit(string LayerName, out GameObject item)
    {
        item = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 使用LayerMask进行射线检测，只检测指定层级的物体
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerName)))
        {
            //只检测可用的Card
            item = hit.collider.gameObject;
            return true;
        }
        return false;
    }
    public static void RenewHitting(ref GameObject lastHovered, GameObject currentHovered)
    {
        //击中卡牌的情况
        if (currentHovered != null)
        {
            // 如果与上次悬停的不是同一个
            if (!currentHovered.GetComponent<IHoverable>().IsOnHover())   
            {
                // 取消上次悬停的卡牌效果
                if (lastHovered != null)
                {
                    lastHovered.GetComponent<IHoverable>().OnHoverExit();
                }
                // 设置新悬停卡牌
                lastHovered = currentHovered;
                lastHovered.GetComponent<IHoverable>().OnHoverEnter();
            }
        }
        //没有击中卡牌的情况
        else
        {
            if (lastHovered != null)
            {
                lastHovered.GetComponent<IHoverable>().OnHoverExit();
                lastHovered = null;
            }
        }
    }
}
