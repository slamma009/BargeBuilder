
using UnityEngine;

public static class FloatArrayExtensions
{
    public static float Sample(this float[] fArr, float t)
    {
        int count = fArr.Length;
        if(count == 0 )
        {
            Debug.Log("Unable to sample empty arrays");
            return 0;
        }
        if (count == 1)
            return fArr[0];

        float iFloat = t * (count - 1);
        int idLower = Mathf.FloorToInt(iFloat);
        int idUperr = Mathf.FloorToInt(iFloat + 1);
        if (idUperr > count)
            return fArr[count - 1];
        if (idLower < 0)
            return fArr[0];

        return Mathf.Lerp(fArr[idLower], fArr[idUperr], iFloat - idLower);
    }
}

