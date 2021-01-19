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
        WonderSequence.AddChild(new SetRelaxedState(BB));
        WonderSequence.AddChild(new NewWonderPosition(BB));
        WonderSequence.AddChild(new SquadControl(BB));
        WonderSequence.AddChild(new EyesPeeled(BB));
        WonderSequence.AddChild(new SpawnShip(BB));
        WonderSequence.AddChild(new WaitNode(BB, 2.0f));
        WonderDecorator WonderRoot = new WonderDecorator(WonderSequence, BB);

        
        CompositeNode AttackSequence = new Sequence(BB);
        WonderSequence.AddChild(new SetAttackState(BB));
        AttackSequence.AddChild(new AquireEnemyTarget(BB));
        AttackSequence.AddChild(new PanicSpawn(BB));
        AttackSequence.AddChild(new SquadControl(BB));
        AttackSequence.AddChild(new PursuitNode(BB));
        EnemySpottedConditional AttackRoot = new EnemySpottedConditional(AttackSequence, BB);

        RootChild.AddChild(WonderRoot);
        RootChild.AddChild(AttackRoot);
  

        InvokeRepeating("ExecuteBT", 0.1f, 0.1f);
    }
}
