using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : SteeringBehaviourBase
{

    public override Vector3 Calculate()
    {
        if (SteeringOn)
        {
            ShipController SC = GetComponent<ShipController>();
            float Desire = SC.GetSteeringDesire(ShipController.SteeringDesires.Pursuit);

            if (SC.GetSteeringDesire(ShipController.SteeringDesires.Pursuit) > 0.1)
            {

                Vector3 ToEvader = SC.BB.TargetPosition - SC.transform.position;
                Vector3 DesiredVelocity = Vector3.zero;

                float RelativeHeading = Vector3.Dot(SC.ControlledShip.RB.velocity, SC.BB.TargetVelocity);
                float LookAhead = 0.0f;
                CannonComponent CC = GetComponent<CannonComponent>();

                if (Vector3.Dot(ToEvader, SC.transform.forward) > 0 && RelativeHeading < -0.95)
                {
                    DesiredVelocity = Vector3.Normalize(SC.BB.TargetPosition - transform.position) * SC.ControlledShip.MaxSpeed;
                    if(CC != null)
                    {
                        CC.Firing = true;
                        Debug.Log("FIring Cannons");
                    }

                    return (DesiredVelocity - SC.ControlledShip.RB.velocity) * Desire;
                }
                if (CC != null)
                {
                    CC.Firing = false;
                }

                //going to last known position.
                if (SC.BB.EnemyShip == null)
                {
                    //Debug.LogError("NO SHIP SET TO PURSUE in" + SC.gameObject.name); 
                    LookAhead = ToEvader.magnitude / SC.ControlledShip.MaxSpeed + SC.BB.TargetVelocity.magnitude;

                    Vector3 DesiredTargetPosition = SC.BB.TargetPosition + SC.BB.TargetVelocity * LookAhead;
                    DesiredVelocity = Vector3.Normalize(DesiredTargetPosition - transform.position) * SC.ControlledShip.MaxSpeed;
                    return (DesiredVelocity - SC.ControlledShip.RB.velocity);
                }
                
                LookAhead = ToEvader.magnitude / SC.ControlledShip.MaxSpeed + SC.BB.EnemyShip.RB.velocity.magnitude;

                SC.BB.TargetPosition = SC.BB.EnemyShip.transform.position + SC.BB.EnemyShip.RB.velocity * LookAhead;

                DesiredVelocity = Vector3.Normalize(SC.BB.TargetPosition - transform.position) * SC.ControlledShip.MaxSpeed;

                return (DesiredVelocity - SC.ControlledShip.RB.velocity) * Desire;
            }
        }
        return Vector3.zero;

    }
}
