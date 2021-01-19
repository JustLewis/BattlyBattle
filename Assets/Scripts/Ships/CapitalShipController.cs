using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipController : ShipController
{

    public GameObject PrefabToSpawn;
    public int SquadSize = 10;
    public int SquadMax = 3;

    private int SquadCount = 0;
    private ShipController CurrentSquadLeader;

    public override void SpawnShip()
    {
        if (Squads.Count < SquadMax)
        {
            if (PrefabToSpawn == null)
            {
                Debug.LogError("No ship to spawn in " + this.name);
                return;
            }
            float Zoffset = 8.0f * transform.localScale.x;

            Zoffset = Random.Range(-Zoffset, Zoffset);

            Vector3 SpawnPos = transform.position + new Vector3(4.0f * ControlledShip.transform.localScale.x, 0, Zoffset);
            //GameObject Obj = Instantiate(PrefabToSpawn, SpawnPos, transform.rotation);
            GameObject Obj = Instantiate(PrefabToSpawn, SpawnPos, transform.rotation);
            ShipController SC = Obj.GetComponent<ShipController>();
            
            SC.BB.TargetPosition = BB.TargetPosition;
            SC.BB.TargetVelocity = BB.TargetVelocity;
            SC.BB.EnemyShip = BB.EnemyShip;

            if (SquadCount <= 0)
            {
                Squads.Add(SC);
                SC.gameObject.name = "Squad Leader";
                CurrentSquadLeader = SC;
                SquadCount = SquadSize;
                CurrentSquadLeader = SC;
                SC.BB.TeamID = BB.TeamID;
                SC.TeamColour = TeamColour;
                SC.ControlledShip.SetColour();
            }
            else
            {
                CurrentSquadLeader.Squads.Add(SC);
                SC.BB.TeamID = BB.TeamID;
                SC.TeamColour = TeamColour;
                SC.ControlledShip.SetColour();
                SquadCount--;
            }
        }

    }

    public override void GiveSquadLocation(Vector3 Pos)
    {
        foreach (ShipController SquadMember in Squads)
        {
            SquadMember.BB.TargetPosition = Pos;
            SquadMember.GiveSquadLocation(Pos);
        }
    }
}
