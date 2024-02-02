using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandom2X2PositionByVector3(Vector3 position)
    {
        float xError = Random.Range(-2.0f, 2.0f);
        float yError = Random.Range(-2.0f, 2.0f);

        return position + Vector3.forward * xError + Vector3.right * yError;
    }
}
