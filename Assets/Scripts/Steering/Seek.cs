using System;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviourBase
{
   public override Vector3 Calculate()
    {
        if (SteeringOn)
        {
            ShipController SC = GetComponent<ShipController>();
            float Desire = SC.GetSteeringDesire((int)ShipController.SteeringDesires.Seek);
            if (Desire > 0.2f)
            {
                Vector3 DesiredVelocity = Vector3.Normalize(SC.BB.TargetPosition - transform.position) * SC.ControlledShip.MaxSpeed;

                return (DesiredVelocity - SC.ControlledShip.RB.velocity) * Desire;
            }
            return Vector3.zero;
        }
        return Vector3.zero;
    }

}