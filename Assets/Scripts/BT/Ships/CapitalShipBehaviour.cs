using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapitalShipBehaviour : ShipBehavior
{
    // Start is called before the first frame update
    public override void initialise()
    {
        Selector RootChild = new Selector(BB);
        RootBTNode = RootChild;

        CompositeNode WonderSequence = new Sequence(BB);
        PatrolDecorator WonderRoot = new PatrolDecorator(WonderSequence, BB);
        WonderSequence.AddChild(new NewWonderPosition(BB));
        WonderSequence.AddChild(new SpawnShip(BB));
        WonderSequence.AddChild(new MoveToTarget(BB));
        WonderSequence.AddChild(new EyesPeeled(BB));

        CompositeNode PatrolSequence = new Sequence(BB);
        PatrolDecorator PatrolRoot = new PatrolDecorator(PatrolSequence, BB);
        PatrolSequence.AddChild(new GetPatrolRouteDestination(BB));
        PatrolSequence.AddChild(new MoveToTarget(BB));
        PatrolSequence.AddChild(new EyesPeeled(BB));


        //RootChild.AddChild(PatrolRoot);
        RootChild.AddChild(WonderRoot);

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }
}

public class SpawnShip : BTNode
{
    ShipBlackBoard BB;
    public SpawnShip(MyBlackBoard BBin) : base (BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.SpawnShip();
        Debug.Log("Spawned ship");
        return BTStatus.Success;
    }
}