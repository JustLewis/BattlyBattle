using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float Desire = float.MaxValue;

    public Vector3 Position;

    private void Start()
    {
        Position = GetComponent<Transform>().position;
    }

    public void ShowDesire()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.white * (Desire / float.MaxValue));
    }
}