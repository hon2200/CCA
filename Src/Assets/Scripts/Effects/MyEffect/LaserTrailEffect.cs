using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class LaserTrailEffect : TrailEffect
{
    public override float PlayEffect(Vector3 start, Vector3 end)
    {
        transform.position = start;
        float distance = Vector3.Distance(start, end);
        var main = effect.main;
        var duration = main.duration;
        transform.rotation = Quaternion.LookRotation(end - start);
        effect.Play();
        AudioSource.PlayClipAtPoint(sound, Vector3.zero, AudioManager.Instance.battleVolume);
        Destroy(gameObject, duration);
        return duration;
    }
}
