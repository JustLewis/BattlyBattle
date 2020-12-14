using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTail : MonoBehaviour
{
    public void VisibleTrailSize(float ScaleIn)
    {
        Vector3 NewScale = transform.localScale;
        NewScale.y = NewScale.x * ScaleIn; //x will always be relative the size of the ship that this tail belongs to.
        transform.localScale = NewScale;
    }
}
