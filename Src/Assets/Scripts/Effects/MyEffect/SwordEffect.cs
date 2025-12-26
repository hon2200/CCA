using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SwordEffect : TrailEffect
{

    public float speed = 5f;
    public GameObject Trail;
    public float waitTime = 0.15f;
    public Quaternion originalRotation;
    public void Awake()
    {
        originalRotation = transform.rotation;
    }

    public override float PlayEffect(Vector3 start, Vector3 end)
    {
        transform.position = start;
        transform.rotation = Quaternion.LookRotation(end - start);

        float distance = Vector3.Distance(start, end);
        float travelTime = distance / speed;

        var main = effect.main;

        StartCoroutine(MoveAndImpact(end, travelTime));

        return travelTime + waitTime;
    }

    private IEnumerator MoveAndImpact(Vector3 target, float travelTime)
    {
        Vector3 startPos = transform.position;
        float t = 0f;

        // ---- Move toward target ----
        while (t < travelTime)
        {
            t += Time.deltaTime;
            float alpha = t / travelTime;

            transform.position = Vector3.Lerp(startPos, target, alpha);
            transform.rotation = Quaternion.LookRotation(target - transform.position);

            yield return null;
        }
        yield return new WaitForSeconds(waitTime);

        transform.position = target;
        transform.rotation = originalRotation;
        // ---- Impact ----
        Trail.SetActive(false);
        effect.transform.position = target;
        effect.Play();

        Destroy(gameObject, effect.main.startLifetime.constant);
    }
}