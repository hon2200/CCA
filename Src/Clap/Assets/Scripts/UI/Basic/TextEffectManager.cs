using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextEffectManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;

    private string currentText = "";
    private int currentIndex = 0;
    public float scrollSpeed = 1f; // 新增：滚动速度
    private RectTransform rectTransform;
    private Vector2 firstRectTransform;
    bool scroll = false;

    public void Start()
    {
        rectTransform = textMeshProUGUI.rectTransform;
        firstRectTransform = rectTransform.anchoredPosition;
    }
    public void Show(string message)
    {
        StartCoroutine(ShowText(message));
    }
    
    // 显示文字的协程
    public IEnumerator ShowText(string newText)
    {
        scroll=false;
        rectTransform.anchoredPosition =new Vector2(firstRectTransform.x, firstRectTransform.y);
        currentText = newText;
        currentIndex = 0;

        textMeshProUGUI.text = "";

        while (currentIndex < currentText.Length)
        {
            textMeshProUGUI.text += currentText[currentIndex];
            currentIndex++;
            yield return new WaitForSeconds(0.1f); // 每个字符的显示间隔
        }
        scroll=true;
        StartCoroutine(ScrollText());
    }

    private IEnumerator ScrollText()
    {
        bool iss = false;
        while (scroll)
        {
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
            if (rectTransform.anchoredPosition.y > -150f& !iss)
            {
                yield return new WaitForSeconds(2f);
                iss = true;
            }
                yield return null;
        }
    }
}