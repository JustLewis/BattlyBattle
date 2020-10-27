using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.XR;

public class AlienStateManager<TheClassStateOwner>
{
    private TheClassStateOwner Owner;
    public AlienState<TheClassStateOwner> CurrentState;

    public void Configure(TheClassStateOwner OwnerIn, AlienState<TheClassStateOwner> TheStateOwnerIn)
    {
        Owner = OwnerIn;
        ChangeState(TheStateOwnerIn);
    }

    public void ChangeState(AlienState<TheClassStateOwner> NewStateIn)
    {
        //Debug.Log("Changing state");
        if (CurrentState != null) { CurrentState.Exit(Owner); }
        else { Debug.Log("No current state to exit"); }
        CurrentState = NewStateIn;
        if (CurrentState != null) { CurrentState.Enter(Owner); }
        else { Debug.LogError("No current state to Enter"); }
    }

    public void Update()
    {
        if(CurrentState != null) { CurrentState.Execute(Owner); }
        else { Debug.Log("No state to update from state manager for " + Owner.ToString()); }
    }
}
