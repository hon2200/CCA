using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MovingTrailEffect : TrailEffect
{
    public float speed = 1f;

    public override float PlayEffect(Vector3 start, Vector3 end)
    {
        transform.position = start;
        transform.rotation = Quaternion.LookRotation(end - start);

        float distance = Vector3.Distance(start, end);
        float travelTime = distance / speed;

        effect.Play();

        StartCoroutine(MoveToTarget(end, travelTime));

        return travelTime;
    }

    private IEnumerator MoveToTarget(Vector3 target, float travelTime)
    {
        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < travelTime)
        {
            t += Time.deltaTime;
            float alpha = t / travelTime;

            transform.position = Vector3.Lerp(startPos, target, alpha);

            yield return null;
        }

        transform.position = target;

        // optional: explosion / impact effect here

        Destroy(gameObject);
    }
}
