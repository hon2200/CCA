using UnityEngine;
using System.Collections;

public class LaserProjectile : MonoBehaviour, IPathvfx
{
    public GameObject explosionPrefab;
    public ParticleSystem particleSystem;
    public Vector3 targetPosition;
    private float duration;

    public void show(Vector3 startPos, Vector3 endPos, float duration)
    {
        float distance = Vector3.Distance(startPos, endPos);
        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.startLifetime = duration; 
        mainModule.startSpeed = distance / duration;
        mainModule.gravityModifier = 0f;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        transform.position = startPos;
        targetPosition = endPos;
        transform.rotation = Quaternion.LookRotation(endPos - startPos);

        this.duration = duration;
        particleSystem.Play();
        StartCoroutine(TravelAndExplode());
    }

    private IEnumerator TravelAndExplode(bool doexplode = false)
    {
        yield return new WaitForSeconds(duration);

        if (doexplode && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, targetPosition, transform.rotation);
        }
        Destroy(gameObject);
    }
}