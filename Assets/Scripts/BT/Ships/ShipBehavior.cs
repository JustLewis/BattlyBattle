using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{
    public ShipBlackBoard BB;

    private BTNode RootBTNode;

    public void initialise()
    {
        Selector RootChild = new Selector(BB);
        RootBTNode = RootChild;

        
        CompositeNode PatrolSequence = new Sequence(BB);
        PatrolDecorator PatrolRoot = new PatrolDecorator(PatrolSequence, BB);
        PatrolSequence.AddChild(new GetPatrolRouteDestination(BB));
        PatrolSequence.AddChild(new MoveToTarget(BB));
        PatrolSequence.AddChild(new EyesPeeled(BB));

        RootChild.AddChild(PatrolRoot);

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }

    public void ExecuteBT()
    {
        RootBTNode.Execute();
    }
}

public class GetPatrolRouteDestination : BTNode
{
    private ShipBlackBoard BB;

    public GetPatrolRouteDestination(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        Debug.Log("Getting Next Destination in patrol route");
        if(BB.RouteNodes.Count == 0)
        {
            Debug.LogError("ShipBevahior.cs GetPatrolRouteDestination failed as no route for patrol");
            return BTStatus.Failure;
        }
        if(BB.RouteNodeIterator > BB.RouteNodes.Count - 1)
        {
            BB.RouteNodeIterator = 0;
        }
        BB.TargetPosition = BB.RouteNodes[BB.RouteNodeIterator];
        BB.RouteNodeIterator++;

        return BTStatus.Success;
    }
}

public class MoveToTarget : BTNode
{
    private ShipBlackBoard BB;

    public MoveToTarget(MyBlackBoard BBIn) : base(BBIn)
    {
        BB = (ShipBlackBoard)BBIn;
    }

    public override BTStatus Execute()
    {
        Debug.LogError("Moving to target" + BB.RouteNodeIterator);
        BB.ControlledShip.MoveToTarget();
        return BTStatus.Success;
    }
}

public class EyesPeeled : BTNode
{
    private ShipBlackBoard BB;

    public EyesPeeled(MyBlackBoard BBin) : base (BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        //if enemy visible, stop patrol and stuff
        if((BB.ControlledShip.transform.position - BB.TargetPosition).magnitude < 1.0f)
        {
            Debug.LogError("Returning sucess here");
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}

public class PatrolDecorator : ConditionalDecorator
{
    private ShipBlackBoard BB;

    public PatrolDecorator(BTNode WrappedNode, MyBlackBoard BBin) : base(WrappedNode,BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override bool CheckStatus()
    {
        return true;
    }
}