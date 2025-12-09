using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class EffectManager : MonoSingleton<EffectManager>
{
    public SerializedDictionary<string, TrailEffect> TrailDictionary;
    public SerializedDictionary<string, SpotEffect> SpotDictionary;
    private bool isRunningQueue = false;

    private readonly Queue<Func<float>> effectQueue = new Queue<Func<float>>();
    public void PlayAll()
    {
        if (!isRunningQueue)
            StartCoroutine(RunEffects());
    }

    public void PlayTrailEffect(bool playnow, string id, GameObject origin, GameObject target)
    {
        TrailDictionary.TryGetValue(id, out var effectPrefab);
        var effect = Instantiate(effectPrefab);
        Vector3 originPosition = origin.transform.position;
        Vector3 targetPosition = target.transform.position;
        if (playnow)
            effect.PlayEffect(originPosition, targetPosition);
        else
            effectQueue.Enqueue(() => effect.PlayEffect(originPosition, targetPosition));
    }

    public void PlaySpotEffect(bool playnow, string id, GameObject origin)
    {
        SpotDictionary.TryGetValue(id, out var effectPrefab);
        var effect = Instantiate(effectPrefab);
        Vector3 originPosition = origin.transform.position;
        if (playnow)
            effect.PlayEffect(originPosition, 10);
        else
            effectQueue.Enqueue(() => effect.PlayEffect(originPosition, 10));
    }

    private IEnumerator RunEffects()
    {
        isRunningQueue = true;

        while (effectQueue.Count > 0)
        {
            var effectFunc = effectQueue.Dequeue();

            float duration = effectFunc();  // Play effect & get duration

            yield return new WaitForSeconds(duration);  // Wait for THAT effect to finish
        }

        isRunningQueue = false;
    }
}