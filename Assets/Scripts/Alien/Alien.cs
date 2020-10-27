using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

struct VecMatPair
{
    public Vector3 point;
    public Matrix4x4 matrix;
}

public class Alien : MonoBehaviour
{
    public GameObject Target;
    public AlienStateManager<Alien> FSM;
    public float MoveSpeed = 0.1f;
    public float HungerTime;

    public uint Hunger = 0;
    private float fHungerTimer;

    //public ComputeShader alienShaderAssist;
    //private VecMatPair[] Data = new VecMatPair[5];
    //private ComputeBuffer TheBuffer;

    public void Awake()
    {
        FSM = new AlienStateManager<Alien>();
        FSM.Configure(this, new AttackingState());
    }

    public void Start()
    {
        FSM.Update();

        fHungerTimer = HungerTime;

        //Compute shader stuff.
        //TheBuffer = new ComputeBuffer(Data.Length, 76);
        //TheBuffer.SetData(Data);
        //int a = alienShaderAssist.FindKernel("Multiply");
        //alienShaderAssist.SetBuffer(a, "DataBuffer", TheBuffer);
        //alienShaderAssist.Dispatch(a, Data.Length, 1, 1);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        FSM.Update();
        EventTimer();
    }

    //Private move variables.
    private bool CloseEnough = false;
    public void MoveToTarget()
    {
        if (Target == null)
        {
            Debug.Log("No target to move to");
            return;
        }

        LookToTarget();

        float TheDistance = Vector3.Distance(Target.transform.position, transform.position);

        if(CloseEnough)
        {
            transform.position -= transform.forward * MoveSpeed;
            if (TheDistance > 1.15)
            {
                CloseEnough = false;
            }
            return;
        }
        if (TheDistance < 1.0f)
        {
            transform.position += transform.forward * MoveSpeed * 0.01f;
            if (TheDistance < 0.85f)
            {
                CloseEnough = true;
            }
        }
        else
        {
            transform.position += transform.forward * MoveSpeed;
        }
    }

    private void LookToTarget()
    {
        if (Target == null)
        {
            Debug.Log("No target to move to");
            return;
        }
        transform.LookAt(Target.transform.position);
    }

    private void EventTimer()
    {
        fHungerTimer -= Time.fixedDeltaTime;
        if (fHungerTimer < 0.0f)
        {
            HungerUp();
            fHungerTimer = HungerTime;
        }
    }
    private void HungerUp()
    {
        if(Hunger <= 10) Hunger++;
        //Debug.Log("Hunger is " + Hunger);
    }
    public bool Hungry()
    {
        if (Hunger > 10) { Debug.Log("Hungry triggered"); }
        return Hunger > 10;
    }

    public bool NotHungry()
    {
        if (Hunger <= 4) { Debug.Log("NotHungry triggered"); }
        return Hunger <= 4;
    }
    public void Feed(uint Amount)
    {
        if (Hunger - Amount < 0)
        {
            Hunger = 0;
        }
        else
        {
            Hunger -= Amount;
        }
    }
}
