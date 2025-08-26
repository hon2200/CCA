using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using static UnityEngine.UI.Image;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField]
    public SerializedDictionary<string, GameObject> EffectDictionary;
    public float maxDistance = 100f;

    void Update()
    {
        if(Input.GetKey(KeyCode.S))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(maxDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            GameObject target = new();
            target.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
            if (Input.GetMouseButton(0))
            {
                Shot(gameObject, target);
            }
            if (Input.GetMouseButton(1))
            {
                Defend(target);
            }
            if (Input.GetMouseButton(2))
            {
                Hit(target);
            }
            Destroy(target);
        }
    }

    public void Shot(GameObject origin, GameObject target, Vector3 offset = new Vector3())
    {
        Vector3 vector3 = new Vector3(0,0,4);
        Vector3 targetPosition = target.transform.position + offset+ vector3;
        GameObject projectile = Instantiate(
            EffectDictionary["Bullet"],
            origin.transform.position,
            Quaternion.LookRotation(targetPosition - origin.transform.position)
        );
        IPathvfx ivfx = projectile.GetComponent<IPathvfx>();
        ivfx.show(origin.transform.position+ vector3, targetPosition);
    }
    public void Defend(GameObject target, Vector3 offset = new Vector3())
    {
        Vector3 vector3 = new Vector3(0, 0, 3.5f);
        Vector3 spawnPos = target.transform.position + offset;
        GameObject shield = Instantiate(
            EffectDictionary["Shield"],
            spawnPos,
            Quaternion.identity
        );
        ITargetvfx ivfx = shield.GetComponent<ITargetvfx>();
        ivfx.show(spawnPos + vector3);
    }
    public void Hit(GameObject target, Vector3 offset = new Vector3())
    {
        Vector3 vector3 = new Vector3(0, 0, 4);
        Vector3 hitPoint = target.transform.TransformPoint(offset);
        GameObject effect = Instantiate(
            EffectDictionary["Hit"],
            hitPoint,
            Quaternion.identity
        );
        ITargetvfx ivfx = effect.GetComponent<ITargetvfx>();
        ivfx.show(hitPoint + vector3);
    }
}
