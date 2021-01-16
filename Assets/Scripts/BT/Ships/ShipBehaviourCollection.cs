using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Standard BT node
/// Get a random wonder position relative tothe location of the ship.
/// </summary>
public class NewWonderPosition : BTNode
{
    private ShipBlackBoard BB;
    public NewWonderPosition(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        Debug.Log("Getting new wonder node");

        float WonderOffset = BB.Controller.transform.localScale.x * 50.0f;
        BB.TargetPosition = new Vector3(Random.Range(-WonderOffset, WonderOffset),
            Random.Range(-WonderOffset, WonderOffset),
            Random.Range(-WonderOffset, WonderOffset)) + BB.Controller.transform.position;

        //BB.TargetPosition.y = 0.0f;//Just to keep everyone on one plane
        BB.Controller.CurrentBTNode = "NewWonderPosition";
        return BTStatus.Success;
    }
}

//No longer used
//public class GetPatrolRouteDestination : BTNode
//{
//    private ShipBlackBoard BB;

//    public GetPatrolRouteDestination(MyBlackBoard BBin) : base(BBin)
//    {
//        BB = (ShipBlackBoard)BBin;
//    }

//    public override BTStatus Execute()
//    {
//        Debug.LogError("Patrol nodes no longer used. Should not be able to get to this behaviour (GetPatrolRouteDestination)");
//        if (BB.RouteNodes.Count == 0)
//        {
//            Debug.LogError("ShipBevahior.cs GetPatrolRouteDestination failed as no route for patrol");
//            return BTStatus.Failure;
//        }
//        if (BB.RouteNodeIterator > BB.RouteNodes.Count - 1)
//        {
//            BB.RouteNodeIterator = 0;
//        }
//        BB.TargetPosition = BB.RouteNodes[BB.RouteNodeIterator];
//        BB.TargetDirection = Vector3.Normalize(bb.TargetPosition - BB.Controller.transform.position);
//        BB.RouteNodeIterator++;

//        //Returning success here to move onto next tree node.
//        return BTStatus.Success;
//    }
//}

/// <summary>
/// Standard BT node, Sets the position to move to and moves on.
/// </summary>
public class MoveToTarget : BTNode
{
    private ShipBlackBoard BB;

    public MoveToTarget(MyBlackBoard BBIn) : base(BBIn)
    {
        BB = (ShipBlackBoard)BBIn;
    }

    public override BTStatus Execute()
    {
        BB.Controller.CurrentBTNode = "MoveToTarget";
        //Debug.LogError("Moving to target" + BB.RouteNodeIterator);
        BB.TargetDirection = Vector3.Normalize(BB.TargetPosition - BB.Controller.transform.position);
        BB.Controller.MoveToTarget();
        return BTStatus.Success;
    }
}

/// <summary>
/// Standar BT node. Runs while moving to the target position, eyes peeled meaning if it detects an emeny, it exists this sequence.
/// </summary>
public class EyesPeeled : BTNode
{
    private ShipBlackBoard BB;

    public EyesPeeled(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.CurrentBTNode = "Eyes peeled";
        BB.TargetDirection = Vector3.Normalize(BB.TargetPosition - BB.Controller.transform.position); //Updating direction to target.

        //if enemy visible, stop patrol and stuff
        PerceptionRadar PR = BB.GetComponent<PerceptionRadar>();

        if (PR != null)
        {
            if (PR.EnemyDetected)
            {
                Debug.Log("Enemy detected, returning failing and setting state to attack.");
                BB.Controller.FSM.ChangeState(new ShipStateAttack());
                return BTStatus.Failure;
            }
        }
        //if reached target node.
        if (Vector3.Distance(BB.TargetPosition, BB.Controller.ControlledShip.transform.position) < BB.Proximity)
        {
            return BTStatus.Success;
        }
        return BTStatus.Running;
    }
}


/// <summary>
/// Ignores all radar contact and pursues enemy ship.
/// </summary>
public class PursuitNode : BTNode
{
    private ShipBlackBoard BB;

    public PursuitNode(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.CurrentBTNode = "Pursuit";
        if (BB.EnemyShip == null)
        {
            Debug.LogError("Supposed to chase enemy but no enemy in BB");
            return BTStatus.Failure;
        }
        if (BB.EnemyShip != null)
        {
            BB.TargetPosition = BB.EnemyShip.transform.position;
            BB.TargetDirection = Vector3.Normalize(BB.TargetPosition - BB.Controller.transform.position);
            Debug.Log("Chasing target");
        }
        //if close to enemy. 
        if (Vector3.Distance(BB.TargetPosition, BB.Controller.ControlledShip.transform.position) < BB.Proximity)
        {
            return BTStatus.Success;
        }
        return BTStatus.Running;
    }
}

/// <summary>
/// Wonder behaviour root. While this is true, keep wondering.
/// </summary>
public class WonderDecorator : ConditionalDecorator
{
    private ShipBlackBoard BB;

    public WonderDecorator(BTNode WrappedNode, MyBlackBoard BBin) : base(WrappedNode, BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override bool CheckStatus()
    {
        PerceptionRadar PR = BB.GetComponent<PerceptionRadar>();
        if(PR == null)
        {
            Debug.LogError("Error getting radar in Wonder Decorator");
            return true; //keep going with current sequence.
        }

        BB.Controller.CurrentBTNode = "patrol Decorator";
        return !PR.EnemyDetected; //If enemy detected, exit wonder behaviour. 
    }
}

public class EnemySpottedConditional : ConditionalDecorator
{
    ShipBlackBoard BB;
    public EnemySpottedConditional(BTNode Wrapped, MyBlackBoard BBin) : base(Wrapped, BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override bool CheckStatus()
    {
        BB.Controller.CurrentBTNode = "EnemySpottedConditional";
        PerceptionRadar PR = BB.Controller.GetComponent<PerceptionRadar>();

        if (PR == null)
        {
            Debug.LogError("No Radar attached to ship");
            return false;
        }

        return PR.EnemyDetected;
    }

}

//one shot BTNode get target
public class AquireEnemyTarget : BTNode
{
    ShipBlackBoard BB;

    public AquireEnemyTarget(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.CurrentBTNode = "Aquire Enemy Target";
        PerceptionRadar PR = BB.GetComponent<PerceptionRadar>();
        if (PR != null)
        {
            BB.EnemyShip = PR.GetEnemyContact();
            if (BB.EnemyShip == null)
            {
                Debug.LogError("Unable to get enemy ship for targetting in AquireEnemy Target");
                return BTStatus.Failure;
            }
            //BB.RouteNodes.Clear(); //no longer using nodes
            BB.TargetPosition = BB.EnemyShip.transform.position;
            BB.TargetSpeed = BB.EnemyShip.RB.velocity.magnitude;

            return BTStatus.Success;
        }
        Debug.LogError("Unable to get perception radar from BB");

        return BTStatus.Failure;

    }
}



public class SpawnShip : BTNode
{
    ShipBlackBoard BB;
    public SpawnShip(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.CurrentBTNode = "SpawningShip";
        BB.Controller.SpawnShip();
        return BTStatus.Success;
    }
}


public class SquadControl : BTNode
{
    ShipBlackBoard BB;
    public SquadControl(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.CurrentBTNode = "SquadControl";
        BB.Controller.GiveSquadLocation(BB.Controller.transform.position);
        return BTStatus.Success;
    }
}

