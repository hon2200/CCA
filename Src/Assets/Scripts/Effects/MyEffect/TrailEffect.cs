using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public ParticleSystem effect;
    public virtual float PlayEffect(Vector3 start, Vector3 end)
    {
        transform.position = start;
        float distance = Vector3.Distance(start, end);
        var main = effect.main;
        var duration = distance / main.startSpeed.constant;
        main.startLifetime = duration;
        transform.rotation = Quaternion.LookRotation(end - start);
        effect.Play();
        Destroy(gameObject, duration);
        return duration;
    }
}
