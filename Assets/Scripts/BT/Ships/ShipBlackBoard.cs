using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBlackBoard : MyBlackBoard
{
    // Start is called before the first frame update
    public ShipController Controller;

    public List<Vector3> RouteNodes;
    public int RouteNodeIterator = 0;
    public float Proximity;
}
