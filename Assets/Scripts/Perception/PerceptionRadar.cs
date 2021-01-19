using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class PerceptionRadar : MonoBehaviour
{
    private float RadarRange;


    public bool RadarMagnifyForDebug;

    [HideInInspector]
    public bool EnemyDetected = false;

    public Dictionary<Ship, RadarMemoryRecord> RadarMem = new Dictionary<Ship, RadarMemoryRecord>();

    public float TimeUntilIgnored = 5.0f;


    void Start()
    {
        RadarRange = GetComponent<ShipController>().ClosestPromity * 15;
        EnemyDetected = false;

        InvokeRepeating("Beep", 0.10f, 0.10f);
    }

    private void OnDrawGizmosSelected()
    {
        Color Col = Color.blue;
        Col.a = 0.3f;
        Gizmos.color = Col;
        Gizmos.DrawSphere(transform.position, RadarRange);
    }
    private void OnDrawGizmos()
    {
        Color Col = Color.black;
        Col.a = 0.025f;
        Gizmos.color = Col;
        Gizmos.DrawSphere(transform.position, RadarRange);
    }

    void Beep()
    {
        float RadarDebugRangeMult = 1.0f;
        if(RadarMagnifyForDebug)
        {
            RadarDebugRangeMult = 100.0f;
            RadarMagnifyForDebug = false;
        }

        int TeamID = GetComponent<ShipBlackBoard>().TeamID;
        Collider[] Targets = Physics.OverlapSphere(transform.position, RadarRange * RadarDebugRangeMult);


        foreach (Collider Targ in Targets)
        {
            Ship SC = Targ.gameObject.GetComponent<Ship>();
            if(SC != null && SC != GetComponent<Ship>())
            {

                RadarMemoryRecord Rec = new RadarMemoryRecord(SC,Time.time, SC.transform.position, SC.RB.velocity, SC.Controller.BB.TeamID);
                if (RadarMem.ContainsKey(SC)) 
                {
                    RadarMem[SC] = Rec;
                }
                else
                {
                    RadarMem.Add(SC,Rec);
                }
            }
        }
        EnemyDetected = false;
        
        foreach (RadarMemoryRecord Rad in RadarMem.Values)
        { 
            float TimeFactor = 1.0f - Mathf.Clamp((Time.time - Rad.TimeLastSeen) / TimeUntilIgnored, 0.0f, 1.0f);
            if (TimeFactor <= 0.0001f) 
            {
                //don't bother doing anything if memory is too old.
                continue;
            }

            if (Rad.LastSeenTeam == TeamID)
            {
                Debug.DrawLine(transform.position, Rad.LastSeenPosition + Rad.LastSeenVelocity * (Rad.TimeLastSeen - Time.time), Color.green * TimeFactor, 0.10f, false);
            }
            else if (Rad.LastSeenTeam != TeamID)
            {
                Debug.DrawLine(transform.position, Rad.LastSeenPosition + Rad.LastSeenVelocity * (Rad.TimeLastSeen - Time.time), Color.red * TimeFactor, 0.10f, false);
                EnemyDetected = true;
            }
        }
    }

    public RadarMemoryRecord GetLastEnemyContact()
    {
        int TeamID = GetComponent <ShipBlackBoard>().TeamID;
        RadarMemoryRecord RetVal = null;
        float TimeSeen = Time.time - 10.0f;
        foreach(RadarMemoryRecord Rad in RadarMem.Values)
        {
            if(Rad.LastSeenTeam != TeamID && Rad.TimeLastSeen > TimeSeen)
            {
                RetVal = Rad;
                TimeSeen = Rad.TimeLastSeen;
            }
        }
        return RetVal;
    }
}


[Serializable]
public class RadarMemoryRecord
{
    [SerializeField]
    public Ship ShipID;

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
        ShipID = null;
        TimeLastSeen = 0.0f;
        LastSeenPosition = Vector3.zero;
        LastSeenVelocity = Vector3.zero;
        LastSeenTeam = -1;
    }

    public RadarMemoryRecord(Ship ship, float time, Vector3 pos, Vector3 vel, int team)
    {
        ShipID = ship;
        TimeLastSeen = time;
        LastSeenPosition = pos;
        LastSeenVelocity = vel;
        LastSeenTeam = team;
    }
}