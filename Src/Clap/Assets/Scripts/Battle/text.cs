using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GetMousePosition : MonoBehaviour
{
    public RectTransform uiElement;
    public Image image;
    public float speed=0.05f;
    private void Start()
    {
        StartCoroutine(HHH());
    }

    public IEnumerator HHH()
    {
        while (true)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiElement,
                Input.mousePosition,
                null,
                out localPoint);
            float distance = localPoint.magnitude;
            image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, distance);
            Vector2 centerPoint = (Vector2.zero + localPoint) / 2f;
            image.rectTransform.localPosition = centerPoint;
            float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
            image.rectTransform.localRotation = Quaternion.Euler(0, 0, angle - 90);
            yield return new WaitForSeconds(speed);
        }
        
    }
}