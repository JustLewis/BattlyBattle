using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerStateManager <StateOwner>
{
    private StateOwner TheOwner;
    private MinerState<StateOwner> CurrentState;

    public void Awake()
    {
        TheOwner = null;
        CurrentState = null;
    }

    public void Configure(StateOwner OwnerIn, MinerState<StateOwner> InitialState)
    {
        TheOwner = OwnerIn;
        ChangeState(InitialState);
    }

    public void ChangeState(MinerState<State>)
}
