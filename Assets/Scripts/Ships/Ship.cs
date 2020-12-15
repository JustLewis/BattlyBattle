﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public ShipBehavior BT;
    public ShipBlackBoard BB;
    //Personalise BB params here.
    public float MaxSpeed;
    public float Health;
    private float Speed;
    
    public Color TeamColour;
    
    private bool Moving = false;

    private MeshRenderer MR;
    private ShipTail shipTail;

    private void Awake()
    {
        MR = GetComponentInChildren<MeshRenderer>();

        MR.material.SetColor("_Color", TeamColour);
    }

    // Start is called before the first frame update
    void Start()
    {
        Speed = MaxSpeed;
        shipTail = GetComponentInChildren<ShipTail>();
        BB = this.gameObject.AddComponent<ShipBlackBoard>();
        BB.ControlledShip = this;
        BB.RouteNodes = new List<Vector3>();
        BB.RouteNodes.Add(Vector3.zero);
        BB.RouteNodes.Add(new Vector3(10.0f,0.0f,10.0f));
        BB.RouteNodes.Add(new Vector3(-10.0f,0.0f,10.0f));
        BB.RouteNodes.Add(new Vector3(10.0f,0.0f,-10.0f));

        BT = this.gameObject.AddComponent<ShipBehavior>();
        BT.BB = BB;
        BT.initialise();

        foreach(WeaponBase Weapon in GetComponentsInChildren<WeaponBase>())
        {
            Weapon.Firing = true;

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Moving)
        {
            Vector3 Direction = BB.TargetPosition - this.transform.position;
            this.transform.forward = Vector3.Normalize(Direction);
            transform.position += transform.forward * 0.05f;
            if(shipTail != null)
            {
                shipTail.VisibleTrailSize(MaxSpeed);
            }
            if(shipTail == null)
            {
                Debug.LogError("Shiptail is missing.");
            }
        }
        else if (!Moving)
        {
            if (shipTail != null)
            {
                shipTail.VisibleTrailSize(Speed);
            }
        }
    }

    public void MoveToTarget()
    {
        Moving = true;
    }

    
    public void Damage(float DamageIn)
    {
        Health -= DamageIn;
        if (Health <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

}
