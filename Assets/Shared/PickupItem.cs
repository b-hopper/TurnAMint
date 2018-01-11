using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        PickUp(other.transform);
    }

    public virtual void OnPickup(Transform item)
    {
        // Nothing for now
    }

    void PickUp(Transform item)
    {
        OnPickup(item);
    }
}
