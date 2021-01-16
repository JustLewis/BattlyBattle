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
        WonderSequence.AddChild(new NewWonderPosition(BB));
        WonderSequence.AddChild(new SquadControl(BB));
        WonderSequence.AddChild(new MoveToTarget(BB));
        WonderSequence.AddChild(new EyesPeeled(BB));
        WonderSequence.AddChild(new SpawnShip(BB));
        PatrolDecorator WonderRoot = new PatrolDecorator(WonderSequence, BB);

        
        CompositeNode AttackSequence = new Sequence(BB);
        AttackSequence.AddChild(new AquireEnemyTarget(BB));
        AttackSequence.AddChild(new SquadControl(BB));
        AttackSequence.AddChild(new PursuitNode(BB));
        EnemySpottedConditional AttackRoot = new EnemySpottedConditional(AttackSequence, BB);

        RootChild.AddChild(WonderRoot);
        RootChild.AddChild(AttackRoot);
  

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }
}

//Some sequences from ShipBehaviour.cs

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


public class SquadControl : BTNode
{
    ShipBlackBoard BB;
    public SquadControl(MyBlackBoard BBin) : base(BBin)
    {
        BB = (ShipBlackBoard)BBin;
    }

    public override BTStatus Execute()
    {
        BB.Controller.GiveSquadLocation(BB.Controller.transform.position);
        return BTStatus.Success;
    }
}

