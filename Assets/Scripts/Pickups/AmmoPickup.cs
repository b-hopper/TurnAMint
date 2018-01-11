using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : PickupItem {

    [SerializeField] EWeaponType weaponType;
    [SerializeField] int amount;
    [SerializeField] float respawnTime;

    public override void OnPickup(Transform item)
    {
        Container playerInventory = item.GetComponentInChildren<Container>();

        GameManager.Instance.Respawner.Despawn(gameObject, respawnTime);

        playerInventory.Put(weaponType.ToString(), amount);

        item.GetComponent<Player>().PlayerShoot.activeWeapon.reloader.HandleOnAmmoChanged();
    }
}
