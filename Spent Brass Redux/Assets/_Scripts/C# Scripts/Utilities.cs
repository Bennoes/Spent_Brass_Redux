using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static float Fuzz(float value, float maxDeviation)
    {
        float deviation = Random.Range(-maxDeviation,maxDeviation);
        return value + deviation;
    }

    public static Vector2 Fuzz(Vector2 value, float maxDeviation)
    {
        
        float x = Fuzz(value.x, maxDeviation);
        float y = Fuzz(value.y, maxDeviation);
        
        return new Vector2(x,y);
    }



}
