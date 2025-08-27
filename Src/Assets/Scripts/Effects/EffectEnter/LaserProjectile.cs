using UnityEngine;
using System.Collections;

public class LaserProjectile : MonoBehaviour, IPathvfx
{
    private float travelSpeed;
    public GameObject explosionPrefab;
    public Vector3 targetPosition;
    private float journeyLength;
    public void show(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        journeyLength = Vector3.Distance(startPos, endPos);
        targetPosition = endPos;
        transform.rotation = Quaternion.LookRotation(endPos - startPos);
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        travelSpeed = particleSystem.main.startSpeed.constant;
        StartCoroutine(TravelAndExplode());
    }
    private IEnumerator TravelAndExplode(bool doexplode = false)
    {
        float travelTime = journeyLength / travelSpeed;
        yield return new WaitForSeconds(travelTime);
        transform.position = targetPosition;
        if (explosionPrefab != null && doexplode)
        {
            Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
