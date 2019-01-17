using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractorConveyor : MonoBehaviour {
    
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();
    public Transform Ramp;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null && !rigid.isKinematic)
        {
            ActiveRigidBodies.Add(rigid);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null && ActiveRigidBodies.Contains(rigid))
        {
            ActiveRigidBodies.Remove(rigid);
        }
    }

    private void FixedUpdate()
    {
        for (var i = ActiveRigidBodies.Count - 1; i >= 0; --i)
        {
            if (ActiveRigidBodies[i] != null)
            {
                Vector3 targetForce = Ramp.up * 2;
                ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce, Time.deltaTime);
            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }
}
