using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class pos : MonoBehaviour
{
    public UIBookmessage message;

    public ScrollRect scrollRect;
    public float scrollSpeed = 0.5f; // 可调节的滚动速度

    private void Start()
    {
        if (scrollRect == null)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScroll != 0)
        {
            // 计算水平偏移量（滚轮上滑时左移，下滑时右移）
            float offsetX = -mouseScroll * scrollSpeed * Time.deltaTime * 1000;

            // 移动Content
            Vector2 newPos = scrollRect.content.anchoredPosition + new Vector2(offsetX, 0);
            scrollRect.content.anchoredPosition = newPos;
        }
    }
}