using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class PerceptionRadar : MonoBehaviour
{
    private float RadarRange;


    public bool ShowRadar;



    [HideInInspector]
    public bool EnemyDetected = false;

    public Dictionary<Ship, RadarMemoryRecord> RadarMem = new Dictionary<Ship, RadarMemoryRecord>();


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

        int TeamID = GetComponent<ShipBlackBoard>().TeamID;
        Collider[] Targets = Physics.OverlapSphere(transform.position, RadarRange);


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
            //As time since last seen goes up, over 10 seconds, set multiplyer to show how old record is.
            float TimeFactor = 1.0f - Mathf.Clamp((Time.time - Rad.TimeLastSeen) / 10.0f, 0.0f, 1.0f);
            if (TimeFactor <= 0.0001f) 
            {
                //don't bother doing anything if memory is too old.
                continue;
            }

            if (Rad.LastSeenTeam == TeamID)
            {
                Debug.DrawLine(transform.position, Rad.LastSeenPosition + Rad.LastSeenVelocity * (Rad.TimeLastSeen - Time.time), Color.green * TimeFactor, 0.10f, false);
                EnemyDetected = true;
            }
            else if (Rad.LastSeenTeam != TeamID)
            {
                Debug.DrawLine(transform.position, Rad.LastSeenPosition + Rad.LastSeenVelocity * (Rad.TimeLastSeen - Time.time), Color.red * TimeFactor, 0.10f, false);
            }
        }
    }

    public Ship GetEnemyContact()
    {
        int TeamID = GetComponent<ShipBlackBoard>().TeamID;
        foreach (RadarMemoryRecord Rad in RadarMem.Values)
        {
            if(Rad.LastSeenTeam != TeamID)
            {
                return Rad.ShipID;
            }
        }
        return null;
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