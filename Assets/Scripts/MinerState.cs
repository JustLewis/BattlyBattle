using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MinerState <Owner>
{
    abstract public void Enter(Owner TheOwner);
    abstract public void Exit(Owner TheOwner);
    abstract public void Update(Owner TheOwner); //Shouldn't interfere with Unity's update as it does not inherit from Mono behavior
}