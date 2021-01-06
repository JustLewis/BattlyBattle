using System;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviourBase
{
   public override Vector3 Calculate()
    {
        ShipController SC = GetComponent<ShipController>();
        float Desire = SC.GetSteeringDesire((int)ShipController.SteeringDesires.Seek);
        if (Desire > 0.1f)
        {
            Vector3 DesiredVelocity = Vector3.Normalize(SC.BB.TargetPosition - transform.position) * SC.ControlledShip.MaxSpeed;

            return (DesiredVelocity - SC.ControlledShip.RB.velocity) * Desire;
        }
        return Vector3.zero;
    }

}