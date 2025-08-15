using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EffectDefinition
{
    public string effectName;
    public GameObject effectPrefab;
    public float duration = 1f;
    public EffectType effectType;
    public bool poolable = true;
    public int poolSize = 10;
}

public enum EffectType
{
    Projectile,
    Shield,
    Impact,
    AreaEffect,
    Other
}

public class EffectManager : MonoSingleton<EffectManager>
{
    [Header("特效定义")]
    public List<GameObject> effectDefinitions = new List<GameObject>();

    private Dictionary<string, GameObject> effectLookup = new Dictionary<string, GameObject>();

    protected void Awake()
    {
        InitializeEffectLookup();
    }
    private void InitializeEffectLookup()
    {
        foreach (var effectDef in effectDefinitions)
        {
            if (effectLookup.ContainsKey(effectDef.name))
            {
                continue;
            }
            effectLookup.Add(effectDef.name, effectDef);
        }
    }

    public void Shot(string effectName, GameObject origin, GameObject target, Vector3 offset)
    {

        Vector3 vector3 = new Vector3(0,0,4);
        Vector3 targetPosition = target.transform.position + offset+ vector3;
        GameObject projectile = Instantiate(
            effectLookup[effectName],
            origin.transform.position,
            Quaternion.LookRotation(targetPosition - origin.transform.position)
        );
        IPathvfx ivfx = projectile.GetComponent<IPathvfx>();
        ivfx.show(origin.transform.position+ vector3, targetPosition);
    }
    public void Defend(string effectName, GameObject target, Vector3 offset)
    {
        Vector3 vector3 = new Vector3(0, 0, 3.5f);
        Vector3 spawnPos = target.transform.position + offset;
        GameObject shield = Instantiate(
            effectLookup[effectName],
            spawnPos,
            Quaternion.identity
        );
        ITargetvfx ivfx = shield.GetComponent<ITargetvfx>();
        ivfx.show(spawnPos + vector3);
    }
    public void Hit(string effectName, GameObject target, Vector3 offset)
    {

        Vector3 vector3 = new Vector3(0, 0, 4);
        Vector3 hitPoint = target.transform.TransformPoint(offset);
        GameObject effect = Instantiate(
            effectLookup[effectName],
            hitPoint,
            Quaternion.identity
        );
        ITargetvfx ivfx = effect.GetComponent<ITargetvfx>();
        ivfx.show(hitPoint + vector3);
    }
    
}
