using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class PerceptionRadar : MonoBehaviour
{
    private float RadarRange;

   //public List<Ship> VisibleTargets; // No need for this.

    public bool ShowRadar;

    //public SphereCollider Radar;

    public Dictionary<Ship, RadarMemoryRecord> RadarMem = new Dictionary<Ship, RadarMemoryRecord>();


    void Start()
    {
        RadarRange = GetComponent<ShipController>().ClosestPromity * 15;
        //Radar.radius = RadarRange;

        InvokeRepeating("Beep", 0.10f, 0.10f);
    }


    private void OnDrawGizmos()
    {
        Color Col = Color.blue;
        Col.a = 0.3f;
        Gizmos.color = Col;
        Gizmos.DrawSphere(transform.position, RadarRange);
    }

    void Beep()
    {
        //VisibleTargets.Clear();

        Collider[] Targets = Physics.OverlapSphere(transform.position, RadarRange);


        foreach (Collider Targ in Targets)
        {
            Ship SC = Targ.gameObject.GetComponent<Ship>();
            if(SC != null)
            {
                RadarMemoryRecord Rec = new RadarMemoryRecord(Time.time, SC.transform.position, SC.RB.velocity, SC.Controller.BB.TeamID);
                if (RadarMem.ContainsKey(SC)) 
                {
                    //Debug.LogError("Updating record");
                    //RadarMem[SC] = Rec;
                }
                else
                {
                    RadarMem.Add(SC,Rec);
                    Debug.Log("Adding record");
                }
                //VisibleTargets.Add(SC);
            }
        }

        int TeamID = GetComponent<ShipBlackBoard>().TeamID;
        int iterator =0;
        foreach (RadarMemoryRecord Rad in RadarMem.Values)
        {
            if(Rad.TimeLastSeen - Time.time > 5.0f)
            {
                //TODO figure out how to remove.
                //RadarMem.Remove(Rad)
            }
            iterator++;
            if (Rad.LastSeenTeam == TeamID)
            {
                Debug.DrawLine(transform.position, Rad.LastSeenPosition + Rad.LastSeenVelocity * (Rad.TimeLastSeen - Time.time), Color.green, 0.10f, false);
                Debug.Log("Drawing Green");
            }
            else if (Rad.LastSeenTeam != TeamID)
            {
                Debug.DrawLine(transform.position, Rad.LastSeenPosition + Rad.LastSeenVelocity * (Rad.TimeLastSeen - Time.time), Color.red, 0.10f, false);
                Debug.Log("Drawing red");
            }
            else
            {
                Debug.Log("Ignoring. Team is " + Rad.LastSeenTeam);
            }
        }
    }
}


[Serializable]
public class RadarMemoryRecord
{
    [SerializeField]
    public float TimeLastSeen;

    [SerializeField]
    public Vector3 LastSeenPosition;

    [SerializeField]
    public Vector3 LastSeenVelocity;

    [SerializeField]
    public int LastSeenTeam;

    public RadarMemoryRecord()
    {
        TimeLastSeen = 0.0f;
        LastSeenPosition = Vector3.zero;
        LastSeenVelocity = Vector3.zero;
        LastSeenTeam = -1;
    }

    public RadarMemoryRecord(float time, Vector3 pos, Vector3 vel, int team)
    {
        TimeLastSeen = time;
        LastSeenPosition = pos;
        LastSeenVelocity = vel;
        LastSeenTeam = team;
    }
}