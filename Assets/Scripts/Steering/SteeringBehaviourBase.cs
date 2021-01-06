using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public abstract class SteeringBehaviourBase : MonoBehaviour
{
    public abstract Vector3 Calculate();
}
