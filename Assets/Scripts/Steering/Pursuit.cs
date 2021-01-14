using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : SteeringBehaviourBase
{
    
    public override Vector3 Calculate()
    {
        ShipController SC = GetComponent<ShipController>();

        if (SC.GetSteeringDesire(ShipController.SteeringDesires.Pursuit) > 0.1)
        {

            Vector3 ToEvader = SC.BB.TargetPosition - SC.transform.position;

            float RelativeHeading = Vector3.Dot(SC.transform.forward, SC.BB.TargetDirection);

            if (Vector3.Dot(ToEvader, SC.transform.forward) > 0 && RelativeHeading < -0.95)
            {
                return GetComponent<Seek>().Calculate();
            }

            if (SC.BB.EnemyShip == null)
            {
                Debug.LogError("NO SHIP SET TO PURSUE in" + SC.gameObject.name);
                return Vector3.zero;
            }

            float LookAhead = ToEvader.magnitude / SC.ControlledShip.MaxSpeed + SC.BB.EnemyShip.RB.velocity.magnitude;

            SC.BB.TargetPosition = SC.BB.EnemyShip.transform.position + SC.BB.EnemyShip.RB.velocity * LookAhead;
            return GetComponent<Seek>().Calculate();
        }

        return Vector3.zero;

    }
}
