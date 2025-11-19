using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text : MonoBehaviour
{
    ////需要通过特效达成传递信息的目的
    ////攻击行动，单独的内容
    ////给枪(shoot- bullet)，双枪(shoot - bullet*2)，激光枪，激光炮(调整一下弹道的参数，Trail的时间，色泽上的处理
    ////RPG，双RPG(实例化一个对象，和给枪一样)
    ////刺，砍，斩（通过粒子系统，生成不同的轨迹），光剑，光刀（调整一下弹道的参数，Trail的时间，色泽上的处理）
    public float maxDistance = 100f;
    public float duration = 1f;
    public GameObject target;

    void Start()
    {
        VFXDictionary.Instance.Load();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(maxDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            target.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
            VFXManager.Instance.PlayEffect(false, "DoubleBullet", gameObject, target);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(maxDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            target.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
            VFXManager.Instance.PlayEffect(false, "LaserCannon", gameObject, target);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(maxDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            target.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
            VFXManager.Instance.PlayEffect(false, "Hit", target);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(maxDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            target.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
            VFXManager.Instance.PlayEffect(false, "Shield", target);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            VFXManager.Instance.PlayAll();
        }
    }
}
