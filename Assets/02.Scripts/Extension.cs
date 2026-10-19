using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class Extension
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return ((1 << layer) & mask) != 0;
    }

    // Axis Mask 해당 축 제외 Vector3 반환
    [Flags] public enum Axis{None = 0,X = 1 << 0,Y = 1 << 1,Z = 1 << 2,}
    public static Vector3 OnlyMask(this Vector3 vec, Axis axisMask)
    {
        float x = (axisMask & Axis.X) != 0 ? vec.x : 0;
        float y = (axisMask & Axis.Y) != 0 ? vec.y : 0;
        float z = (axisMask & Axis.Z) != 0 ? vec.z : 0;
        return new Vector3(x,y,z);
    }
}
