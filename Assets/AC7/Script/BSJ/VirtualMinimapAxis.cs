using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMinimapAxis : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.parent.forward.ToFlatVector());
    }
}

public static class VectorExtender
{
    public static Vector3 ToFlatVector(this Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }
}