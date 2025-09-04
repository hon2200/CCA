using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour, ITargetvfx
{
    public ParticleSystem explosionPS;
    private float duration;

    public void show(Vector3 target, float duration)
    {
        this.duration = duration;
        ParticleSystem.MainModule mainModule = explosionPS.main;
        mainModule.startLifetime = duration;
        explosionPS.Play();
        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}