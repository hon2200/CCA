using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class EffectEvent
{
    //特效持续时间
    public float Duration { get; set; }
    //特效开始调用
    public Action OnStart { get; set; }
    //特效延迟
    public float Delay { get; set; }
}

//我觉得我应该在这里面写一个Dictionary，是string（Effect ID）和具体Effect
//（也就是函数，不过需要做成委托Action）的对应
//原本的EffectDictionary我觉得有点多余，这个把毫不相关的特效预制体组合在一块
//我觉得特效具体实现函数可以下放。不过现在不着急
public class EffectManager : MonoSingleton<EffectManager>
{
    [SerializeField]
    //轨迹预制体
    public SerializedDictionary<string, GameObject> TrailDictionary;
    //原地效果预制体
    public SerializedDictionary<string, GameObject> SpotDictionary;
    public float maxDistance = 100f;

    //这个东西再未来要做成表的，在代码里写String的一一对应关系显然太蠢了
    public void CreateTrailEvent(string EffectID ,GameObject origin, 
        GameObject target,float Delay = 0)
    {
        EffectEvent effectEvent = new EffectEvent();
        switch(EffectID)
        {
            case "Shoot":
                effectEvent.OnStart += () => Shoot("Bullet", origin, target);
                effectEvent.Duration = 0.1f;
                break;
        }
        effectEvent.Delay = Delay;
        EffectToDoList.Instance.EnqueueEffect(effectEvent);
    }
    public void CreateSpotEvent(string EffectID, GameObject target, float Delay = 0)
    {
        EffectEvent effectEvent = new EffectEvent();
        switch(EffectID)
        {
            case "Defend":
                effectEvent.OnStart += () => Spot("Shield", 3.5f, target);
                effectEvent.Duration = 1f;
                break;
            case "Hit":
                effectEvent.OnStart += () => Spot("Hit", 4f, target);
                effectEvent.Duration = 0.2f;
                break;
        }
        effectEvent.Delay = Delay;
        EffectToDoList.Instance.EnqueueEffect(effectEvent);
    }

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
                Shoot("Bullet",gameObject, target);
            }
            Destroy(target);
        }
    }

    //轨迹型特效
    public void Shoot(string TrailPrefabID ,GameObject origin, GameObject target, Vector3 offset = new Vector3())
    {
        Vector3 vector3 = new Vector3(0, 0, 4);
        Vector3 targetPosition = target.transform.position + offset+ vector3;
        GameObject projectile = Instantiate(
            TrailDictionary[TrailPrefabID],
            origin.transform.position,
            Quaternion.LookRotation(targetPosition - origin.transform.position)
        );
        IPathvfx ivfx = projectile.GetComponent<IPathvfx>();
        ivfx.show(origin.transform.position+ vector3, targetPosition);
    }
    //定位型特效
    //order指显示上的优先级，也就是调整位置的z坐标
    public void Spot(string SpotPrefabID ,float order ,GameObject target, Vector3 offset = new Vector3())
    {
        Vector3 vector3 = new Vector3(0, 0, order);
        Vector3 spawnPos = target.transform.position + offset;
        GameObject shield = Instantiate(
            SpotDictionary[SpotPrefabID],
            spawnPos,
            Quaternion.identity
        );
        ITargetvfx ivfx = shield.GetComponent<ITargetvfx>();
        ivfx.show(spawnPos + vector3);
    }
}
