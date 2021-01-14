using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviourBase
{
    
    public override Vector3 Calculate()
    {
        ShipController SC = GetComponent<ShipController>();
        if (SC.GetSteeringDesire(ShipController.SteeringDesires.Flee) > 0.5f)
        {
            Vector3 DesiredVelocity = Vector3.Normalize(SC.transform.position - SC.BB.TargetPosition) * SC.ControlledShip.MaxSpeed;

            return (DesiredVelocity - SC.ControlledShip.RB.velocity) * SC.GetSteeringDesire(ShipController.SteeringDesires.Flee);
        }
        return Vector3.zero;
    }
}
