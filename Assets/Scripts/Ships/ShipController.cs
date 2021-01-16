using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [HideInInspector]
    public ShipBehavior BT;
    [HideInInspector]
    public ShipBlackBoard BB;

    [HideInInspector]
    public List<ShipController> Squads = new List<ShipController>();

    public bool Controlled;

    public Color TeamColour;

    [HideInInspector]
    public Ship ControlledShip;

    public float ClosestPromity;

    public List<Vector3> SquadPositions = new List<Vector3>();

    [HideInInspector]
    public enum SteeringDesires 
    { 
        Seek = 0,
        Arrive = 1,
        Flee = 2,
        Pursuit = 3
    }

    private float[] SteeringDesireFloats = {1.0f,0,0,0};


    public void Awake()
    {
        ControlledShip = GetComponent<Ship>();
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
        float WonderOffset = 10 * transform.localScale.x;
        BB.RouteNodes.Add(new Vector3(WonderOffset, 0.0f, WonderOffset));
        BB.RouteNodes.Add(new Vector3(-WonderOffset, 0.0f, WonderOffset));
        BB.RouteNodes.Add(new Vector3(WonderOffset, 0.0f, -WonderOffset));

        BT = GetComponent<ShipBehavior>();
        if (BT == null)
        {
            Debug.LogError("NO BEHAVIOUR TREE ATTACHED TO THIS OBJECT : " + this.gameObject.name);
            return;
        }

        BT.BB = BB;
    }

    public void Start()
    {
        BT.initialise();
    }

    public void FixedUpdate()
    {
        if (!Controlled)
        {
            MoveToTarget();
        }
        else 
        { 
        //TODO add control over ship if possessed?
        
        }

    }

    public void MoveToTarget()
    {

        SteeringDesireFloats[(int)SteeringDesires.Seek] = Mathf.Clamp(Vector3.Distance(BB.Controller.transform.position, BB.TargetPosition) / (BB.Proximity * 10),0.0f,1.0f);
        SteeringDesireFloats[(int)SteeringDesires.Arrive] = 1.0f - SteeringDesireFloats[(int)SteeringDesires.Seek];

        Vector3 DesiredVelocity = Vector3.zero;
       
        foreach (SteeringBehaviourBase Steering in GetComponents<SteeringBehaviourBase>())
        {
            DesiredVelocity += Steering.Calculate();
        }

        ControlledShip.MoveToTarget(DesiredVelocity);
        
    }

    public virtual void SpawnShip() { }

    public virtual void GiveSquadLocation(Vector3 Pos)
    {
        int Iterator = 0;

        FormationPosition[] FP = GetComponentsInChildren<FormationPosition>();

        foreach (ShipController SquadMember in Squads)
        {
            SquadMember.BB.RouteNodes.Clear();
            SquadMember.BB.RouteNodeIterator = 0;
            //SquadMember.BB.RouteNodes.Add(Pos);
            //SquadMember.BB.TargetPosition = Pos;
            SquadMember.BB.RouteNodes.Add(FP[Iterator].GetPos());
            SquadMember.BB.TargetPosition = FP[Iterator].GetPos();

            Iterator++;
        }
    }

    public float GetSteeringDesire(SteeringDesires In)
    {
        return SteeringDesireFloats[(int)In];
    }

    public void SetSteeringDesire(SteeringDesires Desire, float NewDes)
    {
        SteeringDesireFloats[(int)Desire] = Mathf.Clamp(NewDes,0.0f,1.0f);
    }
}
