using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public ShipBehavior BT;
    public ShipBlackBoard BB;
    //Personalise BB params here.

    
    private bool Moving = false;
    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Moving)
        {
            Vector3 Direction = BB.TargetPosition - this.transform.position;
            this.transform.forward = Vector3.Normalize(Direction);
            transform.position += transform.forward * 0.05f;
        }
    }

    public void MoveToTarget()
    {
        Moving = true;
    }


}

