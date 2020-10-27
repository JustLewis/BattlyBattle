using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AlienState<TheStateOwnerClass>
{
    public ComputeShader StateAssistant;
    public abstract void Enter(TheStateOwnerClass AlienIn);
    public abstract void Exit(TheStateOwnerClass AlienIn);
    public abstract void Execute(TheStateOwnerClass AlienIn);
}
