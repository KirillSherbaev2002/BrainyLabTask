using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speedToKeep;
    [SerializeField] private float calibrationSpeed;

    private void OnValidate()
    {
        if (speedToKeep < 0) speedToKeep = 0;
        if (calibrationSpeed < 0) calibrationSpeed = 0;
    }

    private void FixedUpdate()
    {
        KeepSpeed();
    }

    private void KeepSpeed()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude < speedToKeep)
        {
            GetComponent<Rigidbody>().velocity += GetComponent<Rigidbody>().velocity * calibrationSpeed;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
