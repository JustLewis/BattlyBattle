using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipController : ShipController
{

    public GameObject PrefabToSpawn;

    public List<ShipController> Squads = new List<ShipController>();

    public override void SpawnShip()
    {
        if(PrefabToSpawn == null)
        {
            Debug.LogError("No ship to spawn in " + this.name);
            return;
        }

        Vector3 SpawnPos = transform.position + new Vector3(2.0f * ControlledShip.transform.localScale.x, 0, 0);
        GameObject Obj = Instantiate(PrefabToSpawn, SpawnPos, transform.rotation);
        Squads.Add(Obj.GetComponent<ShipController>());

    }
}
