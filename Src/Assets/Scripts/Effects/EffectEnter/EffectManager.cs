using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class EffectEvent
{
    //��Ч����ʱ��
    public float Duration { get; set; }
    //��Ч��ʼ����
    public Action OnStart { get; set; }
    //��Ч�ӳ�
    public float Delay { get; set; }
}

//�Ҿ�����Ӧ����������дһ��Dictionary����string��Effect ID���;���Effect
//��Ҳ���Ǻ�����������Ҫ����ί��Action���Ķ�Ӧ
//ԭ����EffectDictionary�Ҿ����е���࣬����Ѻ�����ص���ЧԤ���������һ��
//�Ҿ�����Ч����ʵ�ֺ��������·š��������ڲ��ż�
public class EffectManager : MonoSingleton<EffectManager>
{
    [SerializeField]
    //�켣Ԥ����
    public SerializedDictionary<string, GameObject> TrailDictionary;
    //ԭ��Ч��Ԥ����
    public SerializedDictionary<string, GameObject> SpotDictionary;
    public float maxDistance = 100f;

    //���������δ��Ҫ���ɱ�ģ��ڴ�����дString��һһ��Ӧ��ϵ��Ȼ̫����
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

    //�켣����Ч
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
    //��λ����Ч
    //orderָ��ʾ�ϵ����ȼ���Ҳ���ǵ���λ�õ�z����
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
