using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCollision : SteeringBehaviourBase
{

    BoxCollider DetectionBox;
    Vector3 StartDetectionBoxSize = new Vector3(7, 3, 10);//7 is width of ship prefab. 3 the height, 2 the minimum depth of the detection box

    List<GameObject> DetectedObjects = new List<GameObject>();

    public void awake()
    {
        DetectionBox = GetComponent<BoxCollider>();
        DetectionBox.size = StartDetectionBoxSize;
    }
    

    public override Vector3 Calculate()
    {
        if(DetectionBox == null)
        {
            Debug.LogError("No box for collision avoidance in " + this.gameObject.name); 
            return Vector3.zero;
        }
        if (DetectionBox != null)
        {
            ShipController SC = GetComponent<ShipController>();


            Vector3 ShipPos = SC.transform.position;
            //detectiobox size z is minimum detection box size.
            float DetectionDepth = StartDetectionBoxSize.z + (SC.ControlledShip.RB.velocity.magnitude / SC.ControlledShip.MaxSpeed) * StartDetectionBoxSize.z;
            Vector3 DetBoxSize = StartDetectionBoxSize;
            DetBoxSize.z = DetectionDepth;

            DetectionBox.size = DetBoxSize;
            Vector3 DetBoxCentre = Vector3.zero;
            DetBoxCentre.z = DetectionDepth / 2.0f;
            DetectionBox.center = DetBoxCentre;

            Vector3 CollatedSteeringForce = Vector3.zero;

            float CurrentDistance = DetectionDepth;
            foreach(GameObject obj in DetectedObjects)
            {
                Vector3 RelativePosition = obj.transform.position - SC.transform.position;
                float Multiplyer = 1.0f - (DetectionDepth - RelativePosition.magnitude) / DetectionDepth; //Magnitude of force to add
                Vector3 ReflectedDirection = Vector3.Cross(RelativePosition, SC.ControlledShip.RB.velocity).normalized;

                CollatedSteeringForce += ReflectedDirection * Multiplyer;
            }
            //Debug.Log("Avoiding " + DetectedObjects.Count);
            return CollatedSteeringForce;

        }
        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == this.gameObject)
        {
            return;
        }
        DetectedObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == this.gameObject)
        {
            return;
        }
        DetectedObjects.Remove(other.gameObject);
    }
}

