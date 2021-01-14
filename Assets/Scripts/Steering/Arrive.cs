using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviourBase
{

    public override Vector3 Calculate()
    {
        ShipController SC = GetComponent<ShipController>();
        if (SC.GetSteeringDesire(ShipController.SteeringDesires.Arrive) > 0.3f)
        {
            Vector3 ToTarget = SC.BB.TargetPosition - SC.BB.Controller.transform.position;
            float Distance = Vector3.Distance(Vector3.zero, ToTarget);

            float Desire = SC.GetSteeringDesire((int)ShipController.SteeringDesires.Seek);
            if (Distance > 0 && Desire > 0.1f)
            {
                float DecellerationAmount = 0.3f;
                float speed = Distance / (DecellerationAmount * 2.0f); //TODO implement deceleration... 2.0f is deceleration amount for now. 
                speed = Mathf.Min(speed, SC.BB.Controller.ControlledShip.MaxSpeed);
                Vector3 DesiredVelocity = ToTarget * speed / Distance;

                return (DesiredVelocity - SC.BB.Controller.ControlledShip.RB.velocity) * Desire;
            }
            return Vector3.zero;
        }
        return Vector3.zero;
    }

}
