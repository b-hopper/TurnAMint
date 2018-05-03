using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.Networking;

[RequireComponent(typeof(InteractionObject)), RequireComponent(typeof(ItemUI))]
public class ItemBase : MonoBehaviour {

    public string itemName;
    public ItemType itemType;
    public float baseWeight = 1;
    public int amount = 1;
    public float value = 1;
    
    Rigidbody thisRB;
    Collider[] colliders;

    [HideInInspector] public Weapon weapon;
    [HideInInspector] public Ammunition ammo;
    [HideInInspector] public ItemUI UI;

    public bool stackable
    {
        get
        {
            bool ret = false;
            if (itemType == ItemType.Ammo)
            {
                ret = true;
            }
            else if (itemType == ItemType.HealthItem)
            {
                ret = true;
            }
            else if (itemType == ItemType.Throwable)
            {
                ret = true;
            }
            
            return ret;
        }
    }

    public float totalWeight
    {
        get
        {
            return amount * baseWeight;
        }
    }

    internal StateManager state;
    [HideInInspector] public InteractionObject interactionObject;

    internal virtual void Awake()
    {
        interactionObject = GetComponent<InteractionObject>();
        thisRB = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        state = GetComponentInParent<StateManager>();
        UI = GetComponent<ItemUI>();
    }

    internal virtual void Start()
    {
        if (this is Weapon)
        {
            itemType = ItemType.Weapon;
            weapon = GetComponent<Weapon>();
        }
        else if (this is Ammunition)
        {
            itemType = ItemType.Ammo;
            ammo = GetComponent<Ammunition>();
        }
    }
    
    public void OnPickupTriggered()
    {
        if (state != null)
        {
            state.PickUp();
            EnableColliders(false);
        }
    }

    public void EnableColliders(bool val)
    {
        if (thisRB != null)
        {
            thisRB.isKinematic = !val;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = val;
        }
    }

    public void OnDrop()
    {
        EnableColliders(true);
    }
}

public enum ItemType
{
    Ammo = 0,
    Weapon = 1,
    HealthItem = 2,
    Armor = 3,
    WeaponMod = 4,
    Throwable = 5,
    Cosmetic = 6
}
