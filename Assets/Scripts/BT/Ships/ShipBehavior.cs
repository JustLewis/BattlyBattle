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
        WonderDecorator WonderRoot = new WonderDecorator(WonderSequence, BB);
        WonderSequence.AddChild(new NewWonderPosition(BB));
        WonderSequence.AddChild(new EyesPeeled(BB));
        WonderSequence.AddChild(new WaitNode(BB));

        CompositeNode AttackSequence = new Sequence(BB);
        EnemySpottedConditional AttackRoot = new EnemySpottedConditional(AttackSequence, BB);
        AttackSequence.AddChild(new AquireEnemyTarget(BB));
        AttackSequence.AddChild(new SquadControl(BB));
        AttackSequence.AddChild(new PursuitNode(BB));

        RootChild.AddChild(WonderRoot);
        RootChild.AddChild(AttackRoot);

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }

    public void ExecuteBT()
    {
        RootBTNode.Execute();
    }
}
