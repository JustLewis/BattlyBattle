using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetRelaxedState : BTNode
{
    private ShipBlackBoard BB;

    public SetRelaxedState(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.FSM.ChangeState(new ShipStateRelax());
        return BTStatus.Success;
    }
}

public class SetAttackState : BTNode
{
    private ShipBlackBoard BB;

    public SetAttackState(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.FSM.ChangeState(new ShipStateAttack());
        return BTStatus.Success;
    }
}

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

        Vector3 DesiredPos = new Vector3(Random.Range(-WonderOffset, WonderOffset),
            Random.Range(-WonderOffset, WonderOffset),
            Random.Range(-WonderOffset, WonderOffset)) + BB.Controller.transform.position;

        BB.Controller.SetBBTarget(null, DesiredPos, Vector3.zero);
        BB.Controller.CurrentBTNode = "NewWonderPosition";
        BB.Controller.FSM.ChangeState(new ShipStateRelax());
        return BTStatus.Success;
    }
}

//Standard node, wait for random time between 1 and 5 or set time
public class WaitNode : BTNode
{
    private ShipBlackBoard BB;

    float TimeToStopWaiting;

    public WaitNode(MyBlackBoard BBIn, float TimeToWaitInSecs) : base (BBIn)
    {
        BB = (ShipBlackBoard)BBin;
        BB.Controller.FSM.ChangeState(new ShipStateRelaxStop());
        TimeToStopWaiting = Time.time + TimeToWaitInSecs;
    }
    public WaitNode(MyBlackBoard BBIn) : base(BBIn)
    {
        BB = (ShipBlackBoard)BBin;
        BB.Controller.FSM.ChangeState(new ShipStateRelaxStop());
        TimeToStopWaiting = Time.time + Random.Range(1.0f, 5.0f);
    }

    public override BTStatus Execute()
    {
        PerceptionRadar PR = BB.GetComponent<PerceptionRadar>();

        BTStatus RetValue = BTStatus.Failure;
        if(TimeToStopWaiting <= Time.time)
        {
            RetValue = BTStatus.Success;
            return RetValue;
        }
        else if (PR != null)
        {
            if (PR.EnemyDetected)
            {
                RetValue = BTStatus.Failure;
                return RetValue;
            }
        }
        else if(TimeToStopWaiting > Time.time)
        { 
            RetValue = BTStatus.Running;
            return RetValue;
        }
        return RetValue;

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
            return BTStatus.Failure; //to exit attack mode.
        }

        PerceptionRadar PR = BB.GetComponent<PerceptionRadar>();
        if(PR == null)
        {
            Debug.LogError("No radar attached to ship.");
        }
        if(PR != null)
        {
            RadarMemoryRecord RMR;
            if(PR.RadarMem.TryGetValue(BB.EnemyShip,out RMR))
            {
                if (RMR.TimeLastSeen + PR.TimeUntilIgnored < Time.time)
                {
                    Debug.Log("Time last seen contact too long ago");
                    BB.Controller.SetBBTargetRadarOnly(RMR);
                    return BTStatus.Success;
                }
            }
        }
        //if close to enemy. 
        if (Vector3.Distance(BB.TargetPosition, BB.Controller.ControlledShip.transform.position) < BB.Proximity)
        {
            return BTStatus.Success;
        }
        BB.Controller.SpawnShip();//just for capital ship.
        return BTStatus.Running;
    }
}

public class PanicSpawn : BTNode
{
    ShipBlackBoard BB;

    public PanicSpawn(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        //Rubbish way to get capital ship to spawn ships for assault.
        for (int i = 0; i < 10; i++)
        {
            BB.Controller.SpawnShip();
        }

        return BTStatus.Success;
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

        BB.Controller.CurrentBTNode = "Wonder Decorator";
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
            RadarMemoryRecord RMR = BB.Controller.GetLastEnemyContact();
            if (RMR == null)
            {
                Debug.LogError("Unable to get enemy ship for targetting in AquireEnemy Target");
                return BTStatus.Failure;
            }
            BB.Controller.SetBBTarget(RMR);

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

