using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IPathvfx
{
    public void show(Vector3 origin, Vector3 target,float duration);

}

public interface ITargetvfx
{
    public void show(Vector3 target, float duration);

}

