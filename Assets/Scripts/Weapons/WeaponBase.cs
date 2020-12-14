using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ProjectilePrefab;

    public float ProjectileSpeed;
    public float ShootSpeed;
    public bool Firing;
    protected float LastShotFired;

    public virtual void Start()
    {
        Projectile Proj =  ProjectilePrefab.GetComponent<Projectile>();
        Proj.SetShotSpeed(ProjectileSpeed);
        
        LastShotFired = 0.0f;

    }

    public virtual void FixedUpdate()
    {
        if (Firing)
        {
            LastShotFired -= Time.fixedDeltaTime;
            if (LastShotFired <= 0.0f)
            {
                LastShotFired = ShootSpeed;
                Debug.Log("Started firing");
                Fire();
            }
        }
        if(LastShotFired >= 0.0f)
        {
            LastShotFired -= Time.fixedDeltaTime;
        }
    }

    public virtual GameObject Fire()
    {

        GameObject Obj = Instantiate(ProjectilePrefab,transform.position, GetInnaccuracy(0.1f));

        return Obj;
    }

    protected Quaternion GetInnaccuracy(float InaccuracyAmount)
    {
        Vector3 OffsetDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        OffsetDirection = transform.forward + OffsetDirection * InaccuracyAmount;

        return Quaternion.LookRotation(OffsetDirection);
    }
}
