using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MultipleTrailEffect : TrailEffect
{
    public int number = 1;
    public float interval = 0.15f;
    public float spacing = 2f;

    public override float PlayEffect(Vector3 start, Vector3 end)
    {
        var main = effect.main;

        float distance = Vector3.Distance(start, end);
        float duration = distance / main.startSpeed.constant;

        float totalTime = (number - 1) * interval + duration;

        StartCoroutine(PlayMultiple(start, end, duration));

        return totalTime;
    }

    private IEnumerator PlayMultiple(Vector3 start, Vector3 end, float duration)
    {
        var main = effect.main;

        Vector3 forward = (end - start).normalized;

        Vector3 right = Vector3.Cross(forward, Vector3.forward);
        right.Normalize();

        main.startLifetime = duration;

        for (int i = 0; i < number; i++)
        {
            float offsetIndex = i - (number - 1) * 0.5f;
            Vector3 offset = right * offsetIndex * spacing;

            Vector3 s = start + offset;
            Vector3 e = end + offset;

            transform.SetPositionAndRotation(
                s,
                Quaternion.LookRotation(e - s)
            );

            effect.Emit(1);

            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
