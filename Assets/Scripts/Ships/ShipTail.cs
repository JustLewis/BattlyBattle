using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTail : MonoBehaviour
{

    public Vector3 StartPos = new Vector3(0,0,3.38f);
    public Vector3 MaxDistPos = new Vector3(0, 0, -1.6f);


    public void VisibleTrailSize(float ScaleIn)
    {
        transform.localPosition = ((1.0f - ScaleIn) * StartPos) + (ScaleIn * MaxDistPos);
    }
}
