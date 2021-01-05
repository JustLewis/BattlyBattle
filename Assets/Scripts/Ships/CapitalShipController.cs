using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipController : ShipController
{

    public GameObject PrefabToSpawn;
    public new void Start()
    {
        BB = this.gameObject.AddComponent<ShipBlackBoard>();
        BB.Controller = this;
        BB.Proximity = ClosestPromity;

        BB.RouteNodes = new List<Vector3>();
        BB.RouteNodes.Add(Vector3.zero);
        BB.RouteNodes.Add(new Vector3(10.0f, 0.0f, 10.0f));
        BB.RouteNodes.Add(new Vector3(-10.0f, 0.0f, 10.0f));
        BB.RouteNodes.Add(new Vector3(10.0f, 0.0f, -10.0f));

        BT = this.gameObject.AddComponent<CapitalShipBehaviour>();
        BT.BB = BB;
        BT.initialise();
    }

    public override void SpawnShip()
    {
        if(PrefabToSpawn == null)
        {
            Debug.LogError("No ship to spawn in " + this.name);
            return;
        }

        Vector3 SpawnPos = transform.position + new Vector3(2.0f * ControlledShip.transform.localScale.x, 0, 0);
        Instantiate(PrefabToSpawn, SpawnPos, transform.rotation);

    }
}
