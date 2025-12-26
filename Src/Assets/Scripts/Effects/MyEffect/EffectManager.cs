using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using TMPro;
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
        if (effectPrefab == null)
        {
            TrailDictionary.TryGetValue("shoot", out var defaultPrefab);
            effectPrefab = defaultPrefab;
        }
        var effect = Instantiate(effectPrefab);
        Vector3 originPosition = origin.transform.position;
        Vector3 targetPosition = target.transform.position;
        if (playnow)
            effect.PlayEffect(originPosition, targetPosition);
        else
        {
            effect.gameObject.SetActive(false);
            effectQueue.Enqueue(() =>
            {
                effect.gameObject.SetActive(true);
                return effect.PlayEffect(originPosition, targetPosition);
            });
        }

    }

    public void PlaySpotEffect(bool playnow, string id, GameObject origin, float number =0)
    {
        SpotDictionary.TryGetValue(id, out var effectPrefab);
        var effect = Instantiate(effectPrefab);
        Vector3 originPosition = origin.transform.position;
        if (playnow)
            effect.PlayEffect(originPosition, 10);
        effect.gameObject.SetActive(false);
        effectQueue.Enqueue(() =>
        {
            effect.gameObject.SetActive(true);
            return effect.PlayEffect(originPosition, 10, number);
        });

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