﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceGraphicsToolkit;

public class Ship : MonoBehaviour
{
    public ShipController Controller;
    public float MaxSpeed;
    public float WarpSpeed;
    public float TurnSpeed;
    private float Speed;
    private float MaxForce;

    public Rigidbody RB;

    public float Health;

    private MeshRenderer MR;
    private ShipTail shipTail;

    private SgtPosition Pos;
    private SgtCameraMove Cam;

    private void Awake()
    {
        MR = GetComponentInChildren<MeshRenderer>();

        RB = GetComponent<Rigidbody>();
        Controller = GetComponent<ShipController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cam = GetComponent<SgtCameraMove>();
        Speed = MaxSpeed;
        shipTail = GetComponentInChildren<ShipTail>();

        //crude way to make a balanced set of movement stuff.
        float ScaleOffset = 2 / transform.localScale.x;
        TurnSpeed = ScaleOffset; 
        MaxSpeed = MaxSpeed * ScaleOffset;
        RB.mass = ScaleOffset * 10;
        MaxForce = ScaleOffset * 10;

        MR.material.SetColor("_Color", Controller.TeamColour);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!Controller.Moving) //slow when not adding any force
        {
            RB.velocity *= 0.9f * Time.deltaTime;
        }

        if (shipTail != null)
        {
            shipTail.VisibleTrailSize(RB.velocity.magnitude / MaxSpeed);
        }
        if (shipTail == null)
        {
            Debug.LogError("Shiptail is missing.");
        }
    }

    public void MoveToTarget(Vector3 Target)
    {
        RotateToTarget();
        RB.velocity += (Target * MaxForce) / RB.mass * Time.deltaTime;
    }

    
    public void Damage(float DamageIn)
    {
        Health -= DamageIn;
        if (Health <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void RotateToTarget()
    {
        //transform.forward = Controller.BB.TargetDirection; //cheating
        transform.forward = RB.velocity; //cheating
    }


}

