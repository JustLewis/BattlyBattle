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

        CompositeNode AttackSequence = new Sequence(BB);
        PatrolDecorator AttackRoot = new PatrolDecorator(AttackSequence, BB);
        AttackSequence.AddChild(new MoveToTarget(BB));
        AttackSequence.AddChild(new SquadControl(BB));
        AttackSequence.AddChild(new PursuitNode(BB));

        //RootChild.AddChild(PatrolRoot);
        RootChild.AddChild(WonderRoot);
        RootChild.AddChild(AttackRoot);

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }

    public void ExecuteBT()
    {
        RootBTNode.Execute();
    }
}
