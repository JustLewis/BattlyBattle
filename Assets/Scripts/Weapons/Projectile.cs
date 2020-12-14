using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float ShotSpeed;
    public float DeathTime;

    private void Start()
    {
        Invoke("DeathTimer", DeathTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * ShotSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ship CollidedShip = collision.gameObject.GetComponent<Ship>();

        if(CollidedShip != null)
        {
            CollidedShip.GetComponent<Ship>().Damage(100.0f);
        }

    }

    public void SetShotSpeed(float ShotSpeedIn)
    {
        ShotSpeed = ShotSpeedIn;
    }

    protected void DeathTimer()
    {
        Destroy(this.gameObject);
    }
}
