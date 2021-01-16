using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipController : ShipController
{

    public GameObject PrefabToSpawn;
    public int SquadSize = 5;
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

            Vector3 SpawnPos = transform.position + new Vector3(4.0f * ControlledShip.transform.localScale.x, 0, 0);
            Debug.Log("Controlled ship local scale x is " + ControlledShip.transform.localScale.x);
            GameObject Obj = Instantiate(PrefabToSpawn, SpawnPos, transform.rotation);
            ShipController SC = Obj.GetComponent<ShipController>();
            if (SquadCount <= 0)
            {
                Squads.Add(SC);
                CurrentSquadLeader = SC;
                SquadCount = SquadSize;
                //StartCoroutine(SetSquadLeaderCR(SC));
                CurrentSquadLeader = SC;
                SC.BB.TeamID = BB.TeamID;
                SC.TeamColour = TeamColour;
                SC.ControlledShip.SetColour();
            }
            else
            {
                //StartCoroutine(SetSquadMemberCR(SC));
                CurrentSquadLeader.Squads.Add(SC);
                SC.BB.TeamID = BB.TeamID;
                SC.TeamColour = TeamColour;
                SC.ControlledShip.SetColour();
                SquadCount--;
            }
        }

    }

    public IEnumerator SetSquadMemberCR(ShipController SCIn)
    {
        bool Tick = false;
        if(!Tick)
        {
            Tick = !Tick;
            yield return new WaitForEndOfFrame();
        }
        CurrentSquadLeader.Squads.Add(SCIn);
        SCIn.BB.TeamID = BB.TeamID;
        SCIn.TeamColour = TeamColour;
        SCIn.ControlledShip.SetColour();
        yield return null;
    }

    public IEnumerator SetSquadLeaderCR(ShipController SCIn)
    {
        bool Tick = false;
        if (!Tick)
        {
            Tick = !Tick;
            yield return new WaitForEndOfFrame();
        }
        CurrentSquadLeader = SCIn;
        SCIn.BB.TeamID = BB.TeamID;
        SCIn.TeamColour = TeamColour;
        SCIn.ControlledShip.SetColour();
        yield return null;
    }
    public override void GiveSquadLocation(Vector3 Pos)
    {
        foreach (ShipController SquadMember in Squads)
        {
            //No longer using nodes.
            //SquadMember.BB.RouteNodes.Clear();
            //SquadMember.BB.RouteNodeIterator = 0;
            //SquadMember.BB.RouteNodes.Add(Pos);
            SquadMember.BB.TargetPosition = Pos;
        }
    }
}
