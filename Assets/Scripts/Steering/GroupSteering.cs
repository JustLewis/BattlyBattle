using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSteering : SteeringBehaviourBase
{
    SphereCollider SCol;

    List<ShipController> ProximityShips = new List<ShipController>();

    ShipController ShipC;

    private int TeamID;

    public void Awake()
    {
        SCol = GetComponent<SphereCollider>();
        ShipC = GetComponent<ShipController>();
    }

    public void Start()
    {
        TeamID = ShipC.BB.TeamID;
        SCol.radius = ShipC.ClosestPromity * transform.localScale.x * 1.8f;
    }
    public override Vector3 Calculate()
    {
        if (SteeringOn)
        {
            if (ProximityShips.Count <= 0)
            {
                return Vector3.zero;
            }
            Vector3 RetVec = (Seperation()* 0.9f);
            RetVec += (Alignment() * 0.8f);
            RetVec += (Cohesion() * 0.6f);

            //Debug.Log("Group desire is " + ShipC.GetSteeringDesire(ShipController.SteeringDesires.Group));
            return RetVec.normalized * ShipC.GetSteeringDesire(ShipController.SteeringDesires.Group);
        }
        return Vector3.zero;
    }

    private Vector3 Seperation()
    {
        Vector3 RetVec = Vector3.zero;
        foreach(ShipController SC in ProximityShips)
        {
            Vector3 ToAgent = transform.position - SC.transform.position;
            RetVec += Vector3.Normalize(ToAgent);
        }
        return RetVec;
    }

    private Vector3 Alignment()
    {
        Vector3 AverageHeading = Vector3.zero;

        foreach(ShipController SC in ProximityShips)
        {
            AverageHeading += SC.ControlledShip.RB.velocity;
        }

        AverageHeading /= (float)ProximityShips.Count;
        return AverageHeading - ShipC.ControlledShip.RB.velocity;
    }

    private Vector3 Cohesion()
    {
        Vector3 CentreOfMass = Vector3.zero, SteeringForce = Vector3.zero;
        foreach(ShipController SC in ProximityShips)
        {
            CentreOfMass += SC.transform.position;
        }
        
        return CentreOfMass /= (float)ProximityShips.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        ShipController SC = other.GetComponent<ShipController>();
        if(SC != null && SC.BB.TeamID== TeamID)
        {
            if(!ProximityShips.Contains(SC))
            {
                ProximityShips.Add(SC);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ShipController SC = other.GetComponent<ShipController>();
        if (SC != null && SC.BB.TeamID == TeamID)
        { 
            ProximityShips.Remove(SC);
        }
    }
}
