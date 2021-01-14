using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBlackBoard : MyBlackBoard
{
    // Start is called before the first frame update
    [HideInInspector]
    public ShipController Controller;

    [HideInInspector]
    public List<Vector3> RouteNodes;
    [HideInInspector]
    public int RouteNodeIterator = 0;
    [HideInInspector]
    public float Proximity;

    public int TeamID;
    [HideInInspector]
    public Ship EnemyShip;

}
