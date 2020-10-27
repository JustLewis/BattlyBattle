using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class food : MonoBehaviour
{
    // Start is called before the first frame update
    public uint FoodAmount;
    CapsuleCollider TheCollider;

    private Color StartColor;

    private IEnumerator corouting;

    void Start()
    {
        TheCollider = GetComponent<CapsuleCollider>();
        TheCollider.isTrigger = true;
        StartColor = GetComponent<Renderer>().material.color;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered the collision");
        Alien AlienComp = other.GetComponent<Alien>();
        if (AlienComp)
        {
            GetComponent<Renderer>().material.color = Color.red;
            corouting = GettingMunched(1, AlienComp);
            StartCoroutine(corouting);
            
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<Renderer>().material.color = StartColor;
        StopCoroutine(corouting);
    }

    IEnumerator GettingMunched(float WaitTime,Alien theAlien)
    {
        while (true)
        {
            theAlien.Feed(5);
            FoodAmount -= 5;
            transform.localScale *= 0.9f;
            if (FoodAmount <= 0)
            {
                Destroy(this.gameObject);
            }
            yield return new WaitForSeconds(0.50f);
        }
    }

}
