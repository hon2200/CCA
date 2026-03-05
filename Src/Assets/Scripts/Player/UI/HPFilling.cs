using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HPFilling : MonoBehaviour
{
    public float distance;
    public SpriteMask Mask;
    public Vector3 OriginalPosition;
    void Start()
    {
        OriginalPosition = Mask.transform.localPosition;
    }
    public void MovingMask(float fillingFactor)
    {
        Mask.transform.localPosition = OriginalPosition - new Vector3(distance * (1 - fillingFactor), 0);
    }
}
