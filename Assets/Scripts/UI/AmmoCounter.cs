using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class AmmoCounter : MonoBehaviour {

    [SerializeField] Text ammoText;
    
    PlayerShoot playerShoot;
    WeaponReloader reloader;
    	
	void Awake () {
        GameManager.Instance.OnLocalPlayerJoined += Instance_OnLocalPlayerJoined;
	}

    private void Instance_OnLocalPlayerJoined(Player player)
    {
        playerShoot = player.PlayerShoot;
        playerShoot.OnWeaponSwitch += HandleOnWeaponSwitch;
    }

    private void HandleOnWeaponSwitch(Weapon activeWeapon)
    {
        reloader = activeWeapon.reloader;
        reloader.OnAmmoChanged += HandleOnAmmoChanged;
        HandleOnAmmoChanged();
    }

    private void HandleOnAmmoChanged()
    {
        int amountInInventory = reloader.RoundsRemainingInInventory;
        int amountInMagazine = reloader.RoundsRemainingInMagazine;

        ammoText.text = string.Format("{0}/{1}", amountInMagazine, amountInInventory);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
