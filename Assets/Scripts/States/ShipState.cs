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
                ShipIn.BB.EnemyShip = PR.GetEnemyContact();
            }
        }
    }

    public override void Execute(ShipController ShipIn)
    {
        ShipIn.MoveToTarget();
        PerceptionRadar PR = ShipIn.GetComponent<PerceptionRadar>();
        if (PR != null || ShipIn.BB.EnemyShip == null)
        {
            if(!PR.EnemyDetected)
            {
                ShipIn.FSM.ChangeState(new ShipStateRelax()); //All ships destroyed or no longer in contact range.
            }
        }
    }

    public override void Exit(ShipController ShipIn)
    {
        Debug.Log("Exiting Attacking state");
        //throw new System.NotImplementedException();
    }

}

public class ShipStateRelax : ShipState<ShipController>
{
    public override void Enter(ShipController ShipIn)
    {
        ShipIn.CurrentState = "Relax";
        ShipIn.BB.EnemyShip = null;
    }

    public override void Execute(ShipController ShipIn)
    {
        ShipIn.MoveToTarget();
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