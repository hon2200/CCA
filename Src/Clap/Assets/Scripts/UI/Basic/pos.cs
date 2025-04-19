using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class pos : MonoBehaviour
{
    public UIBookmessage message;

    public ScrollRect scrollRect;
    public float scrollSpeed = 0.5f; // �ɵ��ڵĹ����ٶ�

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
            // ����ˮƽƫ�����������ϻ�ʱ���ƣ��»�ʱ���ƣ�
            float offsetX = -mouseScroll * scrollSpeed * Time.deltaTime * 1000;

            // �ƶ�Content
            Vector2 newPos = scrollRect.content.anchoredPosition + new Vector2(offsetX, 0);
            scrollRect.content.anchoredPosition = newPos;
        }
    }
}