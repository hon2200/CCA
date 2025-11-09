using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class HoverableBase : MonoBehaviour, IHoverable
{
    protected Quaternion rotationOrigin;
    protected Vector3 positionOrigin;
    protected Vector3 scaleOrigin;
    protected bool onHover;

    public bool IsOnHover() => onHover;

    public virtual void OnHoverEnter(Vector3? scaleMultiplier, Quaternion? rotationOffset, Vector3? positionOffset,
                                        Quaternion? rotationFinal, Vector3? positionFinal)
    {
        rotationOrigin = transform.rotation;
        positionOrigin = transform.position;
        scaleOrigin = transform.localScale;

        if (scaleMultiplier.HasValue)
            transform.localScale = Vector3.Scale(transform.localScale, scaleMultiplier.Value);

        if (rotationOffset.HasValue)
            transform.rotation *= rotationOffset.Value;

        if (positionOffset.HasValue)
            transform.position += positionOffset.Value;

        if (rotationFinal.HasValue)
            transform.rotation = rotationFinal.Value;

        if (positionFinal.HasValue)
            transform.position = positionFinal.Value;

        onHover = true;
    }

    public virtual void OnHoverExit()
    {
        transform.rotation = rotationOrigin;
        transform.position = positionOrigin;
        transform.localScale = scaleOrigin;
        onHover = false;
    }
}


//悬停接口
public interface IHoverable
{
    bool IsOnHover();
    void OnHoverEnter(Vector3? scaleMultiplier = null, Quaternion? rotationOffset = null, Vector3? positionOffset = null,
        Quaternion? rotationFinal = null, Vector3? positionFinal = null);
    void OnHoverExit();
}