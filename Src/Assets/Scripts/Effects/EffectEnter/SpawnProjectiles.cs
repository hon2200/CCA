using UnityEngine;

public  class LaserEmitter : MonoBehaviour
{
    public float maxDistance = 100f;
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            FireLaser1();
        }
        if (Input.GetMouseButton(1))
        {
            FireLaser2();
        }
        if (Input.GetMouseButton(2))
        {
            FireLaser3();
        }
    }

    public void FireLaser1()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(maxDistance);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPosition = hit.point;
        }
        GameObject targe =new();
        targe.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
        Effect.Shot(this.gameObject,targe);
        Destroy(targe);
    }
    public void FireLaser2()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(maxDistance);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPosition = hit.point;
        }
        GameObject targe = new();
        targe.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
        Effect.Defend( targe);
        Destroy(targe);
    }
    public void FireLaser3()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(maxDistance);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPosition = hit.point;
        }
        GameObject targe = new();
        targe.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
        Effect.Hit(targe);
        Destroy(targe);
    }
}