using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AttackDefendingEffect : SpotEffect
{
    public TextMeshPro text;
    public override float PlayEffect(Vector3 position, float duration, float number)
    {
        transform.position = position;
        duration = 1f;
        text.text = number.ToString();
        Destroy(gameObject, duration);
        return duration;
    }
}
