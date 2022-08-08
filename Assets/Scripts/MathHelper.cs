using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public static float RemapAndLimitToRange(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var num = Remap(from, fromMin, fromMax, toMin, toMax);

        if (num > toMax)
            num = toMax;
        if (num < toMin)
            num = toMin;

        return num;
    }
}
