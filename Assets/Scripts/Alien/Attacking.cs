using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AttackingState : AlienState<Alien>
{

    public override void Enter(Alien AlienIn)
    {
        if(AlienIn.Target && AlienIn.Target.tag == "Temp")
        {
            return;
        }
        else 
        {
            GameObject[] ObjectList = GameObject.FindGameObjectsWithTag("Temp");
            AlienIn.Target = ObjectList[0];
        }
    }

    public override void Execute(Alien Ownerin)
    {
        Ownerin.MoveToTarget();
        if(Ownerin.Hungry())
        {
            Ownerin.FSM.ChangeState(new HungryState());
        }
    }

    public override void Exit(Alien AlienIn)
    {
        Debug.Log("Exiting state");
        //throw new System.NotImplementedException();
    }
}
