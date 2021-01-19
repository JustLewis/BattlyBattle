using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipState<StateOwnerClass>
{
    public abstract void Enter(StateOwnerClass AlienIn);
    public abstract void Exit(StateOwnerClass AlienIn);
    public abstract void Execute(StateOwnerClass AlienIn);
}


public class ShipStateAttack : ShipState<ShipController>
{
    public override void Enter(ShipController ShipIn)
    {
        ShipIn.CurrentState = "Attack";
        if (ShipIn.BB.EnemyShip != null)
        {
            return;
        }
        else
        {
            PerceptionRadar PR = ShipIn.GetComponent<PerceptionRadar>();
            if(PR != null)
            {
                //ShipIn.BB.EnemyShip = PR.GetEnemyContact(); //This should be set in behaviour tree.
            }
        }

        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Pursuit, 1.0f);
        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Seek, 0.2f);

    }

    public override void Execute(ShipController ShipIn)
    {
        ShipIn.UpdateBB();
        ShipIn.MoveToTargetAttack();

        //States should be changed from BT. 
        //PerceptionRadar PR = ShipIn.GetComponent<PerceptionRadar>();
        //if (PR != null || ShipIn.BB.EnemyShip == null)
        //{
        //    if (!PR.EnemyDetected)
        //    {
        //        ShipIn.FSM.ChangeState(new ShipStateRelax()); //All ships destroyed or no longer in contact range.
        //    }
        //}
    }

    public override void Exit(ShipController ShipIn)
    {
        Debug.Log("Exiting Attacking state");
    }

}

public class ShipStateRelax : ShipState<ShipController>
{
    public override void Enter(ShipController ShipIn)
    {
        ShipIn.CurrentState = "Relax";
        ShipIn.BB.EnemyShip = null;
        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Pursuit, 0.0f);
        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Group, 0.2f);
        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Seek, 0.2f);
    }

    public override void Execute(ShipController ShipIn)
    {
        ShipIn.MoveToTargetRelaxed();

        //States should be changed from BT. 
        PerceptionRadar PR = ShipIn.GetComponent<PerceptionRadar>();
        if (PR != null)
        {
            if (PR.EnemyDetected)
            {
                ShipIn.FSM.ChangeState(new ShipStateAttack());
            }
        }
    }

    public override void Exit(ShipController ShipIn)
    {
        Debug.Log("Exiting Relaxed state");
        //throw new System.NotImplementedException();
    }
}

public class ShipStateRelaxStop : ShipState<ShipController>
{ 
    public override void Enter(ShipController ShipIn)
    {
        ShipIn.CurrentState = "RelaxedStop";
        ShipIn.BB.EnemyShip = null;

        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Group, 0.2f);
        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Pursuit, 0.0f);
        ShipIn.SetSteeringDesire(ShipController.SteeringDesires.Seek, 0.0f);
    }

    public override void Execute(ShipController ShipIn)
    {
        //States should be changed from BT. 
        PerceptionRadar PR = ShipIn.GetComponent<PerceptionRadar>();
        if (PR != null)
        {
            if (PR.EnemyDetected)
            {
                ShipIn.FSM.ChangeState(new ShipStateAttack());
            }
        }
    }

    public override void Exit(ShipController ShipIn)
    {
        Debug.Log("Exiting RelaxedStop state");
        //throw new System.NotImplementedException();
    }
}