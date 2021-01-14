using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{
    [HideInInspector]
    public ShipBlackBoard BB;

    protected BTNode RootBTNode;

    public virtual void initialise()
    {
        Selector RootChild = new Selector(BB);
        RootBTNode = RootChild;

        CompositeNode WonderSequence = new Sequence(BB);
        PatrolDecorator WonderRoot = new PatrolDecorator(WonderSequence, BB);
        WonderSequence.AddChild(new NewWonderPosition(BB));
        WonderSequence.AddChild(new MoveToTarget(BB));
        WonderSequence.AddChild(new EyesPeeled(BB));
        
        CompositeNode PatrolSequence = new Sequence(BB);
        PatrolDecorator PatrolRoot = new PatrolDecorator(PatrolSequence, BB);
        PatrolSequence.AddChild(new GetPatrolRouteDestination(BB));
        PatrolSequence.AddChild(new MoveToTarget(BB));
        PatrolSequence.AddChild(new EyesPeeled(BB));

        CompositeNode MoveToSequence = new Sequence(BB);
        PatrolDecorator MoveToRoot = new PatrolDecorator(MoveToSequence, BB);
        MoveToSequence.AddChild(new MoveToTarget(BB));
        MoveToSequence.AddChild(new EyesPeeled(BB));

        //RootChild.AddChild(PatrolRoot);
        RootChild.AddChild(WonderRoot);
        RootChild.AddChild(MoveToRoot);
        RootChild.AddChild(PatrolRoot);

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }

    public void ExecuteBT()
    {
        RootBTNode.Execute();
    }
}

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

        BB.TargetPosition.y = 0.0f;//Just to keep everyone on one plane
        return BTStatus.Success;
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
        BB.TargetDirection = Vector3.Normalize(bb.TargetPosition - BB.Controller.transform.position);
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
        //Debug.LogError("Moving to target" + BB.RouteNodeIterator);
        BB.TargetDirection = Vector3.Normalize(bb.TargetPosition - BB.Controller.transform.position);
        BB.Controller.MoveToTarget();
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
        BB.TargetDirection = Vector3.Normalize(bb.TargetPosition - BB.Controller.transform.position);

        //if enemy visible, stop patrol and stuff


        //if reached target node.
        if (Vector3.Distance(BB.TargetPosition, BB.Controller.ControlledShip.transform.position) < BB.Proximity)
        {
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

