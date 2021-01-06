using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [HideInInspector]
    public ShipBehavior BT;
    [HideInInspector]
    public ShipBlackBoard BB;

    public bool Controlled;
    [HideInInspector]
    public bool Moving = false;

    public Color TeamColour;

    [HideInInspector]
    public Ship ControlledShip;

    public float ClosestPromity;

    [HideInInspector]
    public enum SteeringDesires 
    { Seek = 0,
    Arrive = 1,
    Flee = 2
    }

    private float[] SteeringDesireFloats = {1.0f,0,0};


    public void Awake()
    {
        ControlledShip = GetComponent<Ship>();
    }

    public void Start()
    {
        BB = GetComponent<ShipBlackBoard>();
        if (BB == null)
        {
            Debug.LogError("NO BLACK BOARD ATTACHED TO THIS OBJECT : " + this.gameObject.name);
            return;
        }
        BB.Controller = this;
        BB.Proximity = ClosestPromity;

        BB.RouteNodes = new List<Vector3>();
        BB.RouteNodes.Add(Vector3.zero);
        BB.RouteNodes.Add(new Vector3(10.0f, 0.0f, 10.0f));
        BB.RouteNodes.Add(new Vector3(-10.0f, 0.0f, 10.0f));
        BB.RouteNodes.Add(new Vector3(10.0f, 0.0f, -10.0f));

        BT = GetComponent<ShipBehavior>();
        if (BT == null)
        {
            Debug.LogError("NO BEHAVIOUR TREE ATTACHED TO THIS OBJECT : " + this.gameObject.name);
            return;
        }

        BT.BB = BB;
        BT.initialise();
    }

    public void FixedUpdate()
    {
        if (Moving && !Controlled)
        {
            MoveToTarget();
        }

    }

    public void MoveToTarget()
    {

        SteeringDesireFloats[(int)SteeringDesires.Arrive] = Vector3.Distance(BB.Controller.transform.position, BB.TargetPosition) / (BB.Proximity * 10);
        SteeringDesireFloats[(int)SteeringDesires.Seek] = 1 - SteeringDesireFloats[(int)SteeringDesires.Arrive];

        Vector3 DesiredVelocity = Vector3.zero;
       
        foreach (SteeringBehaviourBase Steering in GetComponents<SteeringBehaviourBase>())
        {
            DesiredVelocity += Steering.Calculate();
        }

        ControlledShip.MoveToTarget(DesiredVelocity);
        
        Moving = true;
    }

    public virtual void SpawnShip() { }

    public float GetSteeringDesire(SteeringDesires In)
    {
        return SteeringDesireFloats[(int)In];
    }

    public void SetSteeringDesire(SteeringDesires Desire, float NewDes)
    {
        SteeringDesireFloats[(int)Desire] = Mathf.Clamp(NewDes,0.0f,1.0f);
    }
}
