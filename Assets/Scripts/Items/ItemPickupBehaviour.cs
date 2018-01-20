<<<<<<< HEAD
<<<<<<< HEAD
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
>>>>>>> refs/remotes/origin/master
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
>>>>>>> refs/remotes/origin/master

public class ItemPickupBehaviour : MonoBehaviour {

    public ItemsBase itemToPickup;
<<<<<<< HEAD
<<<<<<< HEAD
    WeaponManager wm;
    Text UItext;
    bool initItem;

    WeaponItem wpToPickup;
    	
	void Start () {
        UItext = CrosshairManager.GetInstance().pickupItemsText;
        wm = GetComponent<WeaponManager>();
        UItext.gameObject.SetActive(false);
	}
	
	
	void Update () {
        CheckItemType();
        ActualPickup();
	}

    private void CheckItemType()
    {
        if (itemToPickup != null)
        {
            if (!initItem)
            {
                UItext.gameObject.SetActive(true);

                switch (itemToPickup.itemType)
                {
                    case ItemsBase.ItemType.weapon:
                        WeaponItemPickup();
                        break;
                    default:
                        break;
                }

                initItem = true;
            }
        }
        else
        {
            if (initItem)
            {
                initItem = false;
                UItext.gameObject.SetActive(false);
            }
        }
    }

    private void ActualPickup()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (wpToPickup != null)
            {
                WeaponReferenceBase targetWeapon = wm.ReturnWeaponWithID(wpToPickup.weaponId);

                if (targetWeapon != null)
                {
                    wm.AvailableWeapons.Add(targetWeapon);

                    if (wm.AvailableWeapons.Count >= wm.maxWeapons + 1)
                    {
                        WeaponReferenceBase prevWeapon = wm.ReturnCurrentWeapon();
                        wm.AvailableWeapons.Remove(prevWeapon);
                        wm.SwitchWeaponWithTargetWeapon(targetWeapon);

                        if (prevWeapon.pickablePrefab != null)
                        {
                            Instantiate(prevWeapon.pickablePrefab,
                                (transform.position + transform.forward * 2) + Vector3.up * 1.5f,
                                Quaternion.identity);
                        }
                    }
                }
                Destroy(wpToPickup.gameObject);
                wpToPickup = null;
                itemToPickup = null;
            }
        }
    }

    private void WeaponItemPickup()
    {
        wpToPickup = itemToPickup.GetComponent<WeaponItem>();

        string targetId = wpToPickup.weaponId;

        if (wm.AvailableWeapons.Count < wm.maxWeapons)
        {
            UItext.text = "Press F to pick up " + targetId;
        }
        else
        {
            UItext.text = "Press F to switch " + wm.ReturnCurrentWeapon().weaponID + " with " + targetId;
        }
    }
=======
=======
>>>>>>> refs/remotes/origin/master
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
<<<<<<< HEAD
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
}
