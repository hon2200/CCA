using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text : MonoBehaviour
{
    public float maxDistance = 100f;
    public float duration = 1f;
    public GameObject target;
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(maxDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            target.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
                EffectManager.Instance.Shoot("Bullet", gameObject, target,duration);
        }
    }
}
