using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public ShipBehavior BT;
    public ShipBlackBoard BB;

    public bool Controlled;

    public bool Moving = false;
    public Color TeamColour;

    public Ship ControlledShip;
    public float ClosestPromity;

    public void Awake()
    {
        ControlledShip = GetComponent<Ship>();
    }

    public void Start()
    {
        BB = this.gameObject.AddComponent<ShipBlackBoard>();
        BB.Controller = this;
        BB.Proximity = ClosestPromity;

        BB.RouteNodes = new List<Vector3>();
        BB.RouteNodes.Add(Vector3.zero);
        BB.RouteNodes.Add(new Vector3(10.0f, 0.0f, 10.0f));
        BB.RouteNodes.Add(new Vector3(-10.0f, 0.0f, 10.0f));
        BB.RouteNodes.Add(new Vector3(10.0f, 0.0f, -10.0f));

        BT = this.gameObject.AddComponent<ShipBehavior>();
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
        if (Vector3.Distance(BB.Controller.transform.position, BB.TargetPosition) < BB.Proximity * 5)
        {
            Debug.Log("Arriving");
            ControlledShip.MoveToTarget(Arrive(BB.TargetPosition,2.0f));
        }
        else
        {
            ControlledShip.MoveToTarget(SeekDir(BB.TargetPosition));
        }
        
        Moving = true;
    }

    public Vector3 SeekDir(Vector3 TargetPos)
    {
        Vector3 DesiredVelocity = Vector3.Normalize(TargetPos - transform.position) * ControlledShip.MaxSpeed;

        return (DesiredVelocity - ControlledShip.RB.velocity);
    }

    public Vector3 Arrive(Vector3 TargetPos,float decelleration)
    {
        Vector3 ToTarget = TargetPos - BB.Controller.transform.position;
        float Distance = Vector3.Distance(Vector3.zero,ToTarget);

        if (Distance > 0)
        {
            float DecellerationAmount = 0.3f;
            float speed = Distance / (DecellerationAmount * decelleration);
            speed = Mathf.Min(speed, BB.Controller.ControlledShip.MaxSpeed);
            Vector3 DesiredVelocity = ToTarget * speed / Distance;

            return (DesiredVelocity - BB.Controller.ControlledShip.RB.velocity);
        }
        return Vector3.zero;
    }

    public virtual void SpawnShip() { }

}
