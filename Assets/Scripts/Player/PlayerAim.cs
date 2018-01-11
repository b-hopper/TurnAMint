using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour {
    
    public void SetRotation(float amount)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - amount, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public float GetAngle()
    {
        return (transform.eulerAngles.x - 180) > 0 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
    }    
}
