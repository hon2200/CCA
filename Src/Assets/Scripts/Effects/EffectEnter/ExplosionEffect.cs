using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ExplosionEffect : MonoBehaviour, ITargetvfx
{
    private ParticleSystem explosionPS;
    private bool hasStarted;

    public void show(Vector3 target)
    {
        explosionPS = GetComponent<ParticleSystem>();
    }

    void Awake()
    {
        explosionPS = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        hasStarted = true;
        explosionPS.Play();
    }

    void Update()
    {
        if (hasStarted && explosionPS.isStopped)
        {
            Destroy(gameObject);
        }
    }
}