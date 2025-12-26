using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;


public class SupplyingEffect : SpotEffect
{
    public TextMeshPro MyText;
    public string originText;

    public float floatHeight = 1f;
    public float scaleMultiplier = 1.5f;
    public void Awake()
    {
        originText = MyText.text;
    }

    public override float PlayEffect(Vector3 position, float duration, float number = 0)
    {
        transform.position = position;
        MyText.text = originText + number;
        // Play particle effect normally
        effect.Play();

        // Use particle lifetime as total time
        float lifetime = effect.main.startLifetime.constant;

        StartCoroutine(AnimateText(lifetime));

        Destroy(gameObject, lifetime);
        return lifetime;
    }

    private IEnumerator AnimateText(float lifetime)
    {
        Vector3 startPos = MyText.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatHeight;

        Vector3 startScale = MyText.transform.localScale;
        Vector3 endScale = startScale * scaleMultiplier;

        TMP_Text tmp = MyText.GetComponent<TMP_Text>();
        Color startColor = tmp.color;

        float t = 0f;

        while (t < lifetime)
        {
            float alpha = t / lifetime;

            MyText.transform.position = Vector3.Lerp(startPos, endPos, alpha);
            MyText.transform.localScale = Vector3.Lerp(startScale, endScale, alpha);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, alpha);
            tmp.color = c;

            t += Time.deltaTime;
            yield return null;
        }
    }
}
