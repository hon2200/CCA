using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;


public class SpotEffect : MonoBehaviour
{
    public ParticleSystem effect;
    public virtual float PlayEffect(Vector3 position, float duration, float number = 0)
    {
        transform.position = position;
        effect.Play();
        Destroy(gameObject, duration);
        return effect.main.startLifetime.constant;
    }
}

