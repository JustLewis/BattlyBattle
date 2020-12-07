using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBlackBoard : MyBlackBoard
{
    // Start is called before the first frame update
    public Ship ControlledShip;

    public List<Vector3> RouteNodes;
    public int RouteNodeIterator = 0;
}
