using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HungryState : AlienState<Alien>
{

    public override void Enter(Alien AlienIn)
    {
        Debug.Log("Entering hungry state");
        if(AlienIn && AlienIn.Target.tag == "Food")
        {
            return;
        }
        else 
        {
            SetHungryTarget(AlienIn);
        }
    }

    public override void Execute(Alien Ownerin)
    {
        if(Ownerin.Target == null) { SetHungryTarget(Ownerin); }
        Ownerin.MoveToTarget();
        if(Ownerin.NotHungry())
        {
            Ownerin.FSM.ChangeState(new AttackingState());
        }
    }

    public override void Exit(Alien AlienIn)
    {
        Debug.Log("Exiting hungry state");
        //throw new System.NotImplementedException();
    }
    private void SetHungryTarget(Alien AlienIn)
    {
        GameObject[] ObjectList = GameObject.FindGameObjectsWithTag("Food");

        AlienIn.Target = ObjectList[0];

    }
}
