using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SpotEffect : MonoBehaviour
{
    public ParticleSystem effect;
    public float PlayEffect(Vector3 position, float duration)
    {
        transform.position = position;
        effect.Play();
        Destroy(gameObject, duration);
        return effect.main.startLifetime.constant;
    }
}