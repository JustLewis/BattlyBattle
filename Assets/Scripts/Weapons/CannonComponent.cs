using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonComponent : WeaponBase
{
    public override void Start()
    {
        base.Start();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override GameObject Fire()
    {
        GameObject Obj = base.Fire();

        return Obj;
    }
}
